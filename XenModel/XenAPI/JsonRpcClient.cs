/*
 * Copyright (c) Citrix Systems, Inc.
 * All rights reserved.
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
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;


namespace XenAPI
{
    public partial class JsonRpcClient
    {
        public Event event_get_record(string session, string _event)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<Event>("event.get_record", new JArray(session, _event ?? ""), serializer);
        }

        public string event_get_by_uuid(string session, string _uuid)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("event.get_by_uuid", new JArray(session, _uuid ?? ""), serializer);
        }

        public long event_get_id(string session, string _event)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<long>("event.get_id", new JArray(session, _event ?? ""), serializer);
        }

        public void event_set_id(string session, string _event, long _id)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("event.set_id", new JArray(session, _event ?? "", _id), serializer);
        }

        public void event_register(string session, string[] _classes)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("event.register", new JArray(session, JArray.FromObject(_classes ?? new string[] {})), serializer);
        }

        public void event_unregister(string session, string[] _classes)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("event.unregister", new JArray(session, JArray.FromObject(_classes ?? new string[] {})), serializer);
        }

        public EventBatch event_from(string session, string[] _classes, string _token, double _timeout)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<EventBatch>("event.from", new JArray(session, JArray.FromObject(_classes ?? new string[] {}), _token ?? "", _timeout), serializer);
        }

        public Session session_get_record(string session, string _session)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<Session>("session.get_record", new JArray(session, _session ?? ""), serializer);
        }

        public XenRef<Session> session_get_by_uuid(string session, string _uuid)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Session>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Session>>("session.get_by_uuid", new JArray(session, _uuid ?? ""), serializer);
        }

        public string session_get_uuid(string session, string _session)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("session.get_uuid", new JArray(session, _session ?? ""), serializer);
        }

        public XenRef<Host> session_get_this_host(string session, string _session)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Host>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Host>>("session.get_this_host", new JArray(session, _session ?? ""), serializer);
        }

        public XenRef<User> session_get_this_user(string session, string _session)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<User>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<User>>("session.get_this_user", new JArray(session, _session ?? ""), serializer);
        }

        public DateTime session_get_last_active(string session, string _session)
        {
            var converters = new List<JsonConverter> {new XenDateTimeConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<DateTime>("session.get_last_active", new JArray(session, _session ?? ""), serializer);
        }

        public bool session_get_pool(string session, string _session)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<bool>("session.get_pool", new JArray(session, _session ?? ""), serializer);
        }

        public Dictionary<string, string> session_get_other_config(string session, string _session)
        {
            var converters = new List<JsonConverter> {new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<string, string>>("session.get_other_config", new JArray(session, _session ?? ""), serializer);
        }

        public bool session_get_is_local_superuser(string session, string _session)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<bool>("session.get_is_local_superuser", new JArray(session, _session ?? ""), serializer);
        }

        public XenRef<Subject> session_get_subject(string session, string _session)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Subject>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Subject>>("session.get_subject", new JArray(session, _session ?? ""), serializer);
        }

        public DateTime session_get_validation_time(string session, string _session)
        {
            var converters = new List<JsonConverter> {new XenDateTimeConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<DateTime>("session.get_validation_time", new JArray(session, _session ?? ""), serializer);
        }

        public string session_get_auth_user_sid(string session, string _session)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("session.get_auth_user_sid", new JArray(session, _session ?? ""), serializer);
        }

        public string session_get_auth_user_name(string session, string _session)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("session.get_auth_user_name", new JArray(session, _session ?? ""), serializer);
        }

        public string[] session_get_rbac_permissions(string session, string _session)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string[]>("session.get_rbac_permissions", new JArray(session, _session ?? ""), serializer);
        }

        public List<XenRef<Task>> session_get_tasks(string session, string _session)
        {
            var converters = new List<JsonConverter> {new XenRefListConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<List<XenRef<Task>>>("session.get_tasks", new JArray(session, _session ?? ""), serializer);
        }

        public XenRef<Session> session_get_parent(string session, string _session)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Session>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Session>>("session.get_parent", new JArray(session, _session ?? ""), serializer);
        }

        public string session_get_originator(string session, string _session)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("session.get_originator", new JArray(session, _session ?? ""), serializer);
        }

        public void session_set_other_config(string session, string _session, Dictionary<string, string> _other_config)
        {
            var converters = new List<JsonConverter> {new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            Rpc("session.set_other_config", new JArray(session, _session ?? "", _other_config == null ? new JObject() : JObject.FromObject(_other_config, serializer)), serializer);
        }

        public void session_add_to_other_config(string session, string _session, string _key, string _value)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("session.add_to_other_config", new JArray(session, _session ?? "", _key ?? "", _value ?? ""), serializer);
        }

        public void session_remove_from_other_config(string session, string _session, string _key)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("session.remove_from_other_config", new JArray(session, _session ?? "", _key ?? ""), serializer);
        }

        public XenRef<Session> session_login_with_password(string _uname, string _pwd)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Session>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Session>>("session.login_with_password", new JArray(_uname ?? "", _pwd ?? ""), serializer);
        }

        public XenRef<Session> session_login_with_password(string _uname, string _pwd, string _version)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Session>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Session>>("session.login_with_password", new JArray(_uname ?? "", _pwd ?? "", _version ?? ""), serializer);
        }

        public XenRef<Session> session_login_with_password(string _uname, string _pwd, string _version, string _originator)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Session>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Session>>("session.login_with_password", new JArray(_uname ?? "", _pwd ?? "", _version ?? "", _originator ?? ""), serializer);
        }

        public void session_logout(string session)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("session.logout", new JArray(session), serializer);
        }

        public void session_change_password(string session, string _old_pwd, string _new_pwd)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("session.change_password", new JArray(session, _old_pwd ?? "", _new_pwd ?? ""), serializer);
        }

        public XenRef<Session> session_slave_local_login_with_password(string _uname, string _pwd)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Session>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Session>>("session.slave_local_login_with_password", new JArray(_uname ?? "", _pwd ?? ""), serializer);
        }

        public XenRef<Session> session_create_from_db_file(string session, string _filename)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Session>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Session>>("session.create_from_db_file", new JArray(session, _filename ?? ""), serializer);
        }

        public XenRef<Task> async_session_create_from_db_file(string session, string _filename)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.session.create_from_db_file", new JArray(session, _filename ?? ""), serializer);
        }

        public void session_local_logout(string session)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("session.local_logout", new JArray(session), serializer);
        }

        public string[] session_get_all_subject_identifiers(string session)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string[]>("session.get_all_subject_identifiers", new JArray(session), serializer);
        }

        public XenRef<Task> async_session_get_all_subject_identifiers(string session)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.session.get_all_subject_identifiers", new JArray(session), serializer);
        }

        public void session_logout_subject_identifier(string session, string _subject_identifier)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("session.logout_subject_identifier", new JArray(session, _subject_identifier ?? ""), serializer);
        }

        public XenRef<Task> async_session_logout_subject_identifier(string session, string _subject_identifier)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.session.logout_subject_identifier", new JArray(session, _subject_identifier ?? ""), serializer);
        }

        public Dictionary<XenRef<Session>, Session> session_get_all_records(string session)
        {
            var converters = new List<JsonConverter> {new XenRefXenObjectMapConverter<Session>()};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<XenRef<Session>, Session>>("session.get_all_records", new JArray(session), serializer);
        }

        public string auth_get_subject_identifier(string session, string _subject_name)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("auth.get_subject_identifier", new JArray(session, _subject_name ?? ""), serializer);
        }

        public Dictionary<string, string> auth_get_subject_information_from_identifier(string session, string _subject_identifier)
        {
            var converters = new List<JsonConverter> {new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<string, string>>("auth.get_subject_information_from_identifier", new JArray(session, _subject_identifier ?? ""), serializer);
        }

        public string[] auth_get_group_membership(string session, string _subject_identifier)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string[]>("auth.get_group_membership", new JArray(session, _subject_identifier ?? ""), serializer);
        }

        public Dictionary<XenRef<Auth>, Auth> auth_get_all_records(string session)
        {
            var converters = new List<JsonConverter> {new XenRefXenObjectMapConverter<Auth>()};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<XenRef<Auth>, Auth>>("auth.get_all_records", new JArray(session), serializer);
        }

        public Subject subject_get_record(string session, string _subject)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<Subject>("subject.get_record", new JArray(session, _subject ?? ""), serializer);
        }

        public XenRef<Subject> subject_get_by_uuid(string session, string _uuid)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Subject>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Subject>>("subject.get_by_uuid", new JArray(session, _uuid ?? ""), serializer);
        }

        public XenRef<Subject> subject_create(string session, Subject _record)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Subject>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Subject>>("subject.create", new JArray(session, _record.ToJObject()), serializer);
        }

        public XenRef<Task> async_subject_create(string session, Subject _record)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.subject.create", new JArray(session, _record.ToJObject()), serializer);
        }

        public void subject_destroy(string session, string _subject)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("subject.destroy", new JArray(session, _subject ?? ""), serializer);
        }

        public XenRef<Task> async_subject_destroy(string session, string _subject)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.subject.destroy", new JArray(session, _subject ?? ""), serializer);
        }

        public string subject_get_uuid(string session, string _subject)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("subject.get_uuid", new JArray(session, _subject ?? ""), serializer);
        }

        public string subject_get_subject_identifier(string session, string _subject)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("subject.get_subject_identifier", new JArray(session, _subject ?? ""), serializer);
        }

        public Dictionary<string, string> subject_get_other_config(string session, string _subject)
        {
            var converters = new List<JsonConverter> {new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<string, string>>("subject.get_other_config", new JArray(session, _subject ?? ""), serializer);
        }

        public List<XenRef<Role>> subject_get_roles(string session, string _subject)
        {
            var converters = new List<JsonConverter> {new XenRefListConverter<Role>()};
            var serializer = CreateSerializer(converters);
            return Rpc<List<XenRef<Role>>>("subject.get_roles", new JArray(session, _subject ?? ""), serializer);
        }

        public void subject_add_to_roles(string session, string _subject, string _role)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Role>()};
            var serializer = CreateSerializer(converters);
            Rpc("subject.add_to_roles", new JArray(session, _subject ?? "", _role ?? ""), serializer);
        }

        public void subject_remove_from_roles(string session, string _subject, string _role)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Role>()};
            var serializer = CreateSerializer(converters);
            Rpc("subject.remove_from_roles", new JArray(session, _subject ?? "", _role ?? ""), serializer);
        }

        public string[] subject_get_permissions_name_label(string session, string _subject)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string[]>("subject.get_permissions_name_label", new JArray(session, _subject ?? ""), serializer);
        }

        public List<XenRef<Subject>> subject_get_all(string session)
        {
            var converters = new List<JsonConverter> {new XenRefListConverter<Subject>()};
            var serializer = CreateSerializer(converters);
            return Rpc<List<XenRef<Subject>>>("subject.get_all", new JArray(session), serializer);
        }

        public Dictionary<XenRef<Subject>, Subject> subject_get_all_records(string session)
        {
            var converters = new List<JsonConverter> {new XenRefXenObjectMapConverter<Subject>()};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<XenRef<Subject>, Subject>>("subject.get_all_records", new JArray(session), serializer);
        }

        public Role role_get_record(string session, string _role)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<Role>("role.get_record", new JArray(session, _role ?? ""), serializer);
        }

        public XenRef<Role> role_get_by_uuid(string session, string _uuid)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Role>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Role>>("role.get_by_uuid", new JArray(session, _uuid ?? ""), serializer);
        }

        public List<XenRef<Role>> role_get_by_name_label(string session, string _label)
        {
            var converters = new List<JsonConverter> {new XenRefListConverter<Role>()};
            var serializer = CreateSerializer(converters);
            return Rpc<List<XenRef<Role>>>("role.get_by_name_label", new JArray(session, _label ?? ""), serializer);
        }

        public string role_get_uuid(string session, string _role)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("role.get_uuid", new JArray(session, _role ?? ""), serializer);
        }

        public string role_get_name_label(string session, string _role)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("role.get_name_label", new JArray(session, _role ?? ""), serializer);
        }

        public string role_get_name_description(string session, string _role)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("role.get_name_description", new JArray(session, _role ?? ""), serializer);
        }

        public List<XenRef<Role>> role_get_subroles(string session, string _role)
        {
            var converters = new List<JsonConverter> {new XenRefListConverter<Role>()};
            var serializer = CreateSerializer(converters);
            return Rpc<List<XenRef<Role>>>("role.get_subroles", new JArray(session, _role ?? ""), serializer);
        }

        public List<XenRef<Role>> role_get_permissions(string session, string _role)
        {
            var converters = new List<JsonConverter> {new XenRefListConverter<Role>()};
            var serializer = CreateSerializer(converters);
            return Rpc<List<XenRef<Role>>>("role.get_permissions", new JArray(session, _role ?? ""), serializer);
        }

        public string[] role_get_permissions_name_label(string session, string _role)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string[]>("role.get_permissions_name_label", new JArray(session, _role ?? ""), serializer);
        }

        public List<XenRef<Role>> role_get_by_permission(string session, string _role)
        {
            var converters = new List<JsonConverter> {new XenRefListConverter<Role>()};
            var serializer = CreateSerializer(converters);
            return Rpc<List<XenRef<Role>>>("role.get_by_permission", new JArray(session, _role ?? ""), serializer);
        }

        public List<XenRef<Role>> role_get_by_permission_name_label(string session, string _label)
        {
            var converters = new List<JsonConverter> {new XenRefListConverter<Role>()};
            var serializer = CreateSerializer(converters);
            return Rpc<List<XenRef<Role>>>("role.get_by_permission_name_label", new JArray(session, _label ?? ""), serializer);
        }

        public List<XenRef<Role>> role_get_all(string session)
        {
            var converters = new List<JsonConverter> {new XenRefListConverter<Role>()};
            var serializer = CreateSerializer(converters);
            return Rpc<List<XenRef<Role>>>("role.get_all", new JArray(session), serializer);
        }

        public Dictionary<XenRef<Role>, Role> role_get_all_records(string session)
        {
            var converters = new List<JsonConverter> {new XenRefXenObjectMapConverter<Role>()};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<XenRef<Role>, Role>>("role.get_all_records", new JArray(session), serializer);
        }

        public Task task_get_record(string session, string _task)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<Task>("task.get_record", new JArray(session, _task ?? ""), serializer);
        }

        public XenRef<Task> task_get_by_uuid(string session, string _uuid)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("task.get_by_uuid", new JArray(session, _uuid ?? ""), serializer);
        }

        public List<XenRef<Task>> task_get_by_name_label(string session, string _label)
        {
            var converters = new List<JsonConverter> {new XenRefListConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<List<XenRef<Task>>>("task.get_by_name_label", new JArray(session, _label ?? ""), serializer);
        }

        public string task_get_uuid(string session, string _task)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("task.get_uuid", new JArray(session, _task ?? ""), serializer);
        }

        public string task_get_name_label(string session, string _task)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("task.get_name_label", new JArray(session, _task ?? ""), serializer);
        }

        public string task_get_name_description(string session, string _task)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("task.get_name_description", new JArray(session, _task ?? ""), serializer);
        }

        public List<task_allowed_operations> task_get_allowed_operations(string session, string _task)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<List<task_allowed_operations>>("task.get_allowed_operations", new JArray(session, _task ?? ""), serializer);
        }

        public Dictionary<string, task_allowed_operations> task_get_current_operations(string session, string _task)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<string, task_allowed_operations>>("task.get_current_operations", new JArray(session, _task ?? ""), serializer);
        }

        public DateTime task_get_created(string session, string _task)
        {
            var converters = new List<JsonConverter> {new XenDateTimeConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<DateTime>("task.get_created", new JArray(session, _task ?? ""), serializer);
        }

        public DateTime task_get_finished(string session, string _task)
        {
            var converters = new List<JsonConverter> {new XenDateTimeConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<DateTime>("task.get_finished", new JArray(session, _task ?? ""), serializer);
        }

        public task_status_type task_get_status(string session, string _task)
        {
            var converters = new List<JsonConverter> {new task_status_typeConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<task_status_type>("task.get_status", new JArray(session, _task ?? ""), serializer);
        }

        public XenRef<Host> task_get_resident_on(string session, string _task)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Host>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Host>>("task.get_resident_on", new JArray(session, _task ?? ""), serializer);
        }

        public double task_get_progress(string session, string _task)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<double>("task.get_progress", new JArray(session, _task ?? ""), serializer);
        }

        public string task_get_type(string session, string _task)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("task.get_type", new JArray(session, _task ?? ""), serializer);
        }

        public string task_get_result(string session, string _task)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("task.get_result", new JArray(session, _task ?? ""), serializer);
        }

        public string[] task_get_error_info(string session, string _task)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string[]>("task.get_error_info", new JArray(session, _task ?? ""), serializer);
        }

        public Dictionary<string, string> task_get_other_config(string session, string _task)
        {
            var converters = new List<JsonConverter> {new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<string, string>>("task.get_other_config", new JArray(session, _task ?? ""), serializer);
        }

        public XenRef<Task> task_get_subtask_of(string session, string _task)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("task.get_subtask_of", new JArray(session, _task ?? ""), serializer);
        }

        public List<XenRef<Task>> task_get_subtasks(string session, string _task)
        {
            var converters = new List<JsonConverter> {new XenRefListConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<List<XenRef<Task>>>("task.get_subtasks", new JArray(session, _task ?? ""), serializer);
        }

        public string task_get_backtrace(string session, string _task)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("task.get_backtrace", new JArray(session, _task ?? ""), serializer);
        }

        public void task_set_other_config(string session, string _task, Dictionary<string, string> _other_config)
        {
            var converters = new List<JsonConverter> {new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            Rpc("task.set_other_config", new JArray(session, _task ?? "", _other_config == null ? new JObject() : JObject.FromObject(_other_config, serializer)), serializer);
        }

        public void task_add_to_other_config(string session, string _task, string _key, string _value)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("task.add_to_other_config", new JArray(session, _task ?? "", _key ?? "", _value ?? ""), serializer);
        }

        public void task_remove_from_other_config(string session, string _task, string _key)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("task.remove_from_other_config", new JArray(session, _task ?? "", _key ?? ""), serializer);
        }

        public XenRef<Task> task_create(string session, string _label, string _description)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("task.create", new JArray(session, _label ?? "", _description ?? ""), serializer);
        }

        public void task_destroy(string session, string _task)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("task.destroy", new JArray(session, _task ?? ""), serializer);
        }

        public void task_cancel(string session, string _task)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("task.cancel", new JArray(session, _task ?? ""), serializer);
        }

        public XenRef<Task> async_task_cancel(string session, string _task)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.task.cancel", new JArray(session, _task ?? ""), serializer);
        }

        public void task_set_status(string session, string _task, task_status_type _value)
        {
            var converters = new List<JsonConverter> {new task_status_typeConverter()};
            var serializer = CreateSerializer(converters);
            Rpc("task.set_status", new JArray(session, _task ?? "", _value.StringOf()), serializer);
        }

        public List<XenRef<Task>> task_get_all(string session)
        {
            var converters = new List<JsonConverter> {new XenRefListConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<List<XenRef<Task>>>("task.get_all", new JArray(session), serializer);
        }

        public Dictionary<XenRef<Task>, Task> task_get_all_records(string session)
        {
            var converters = new List<JsonConverter> {new XenRefXenObjectMapConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<XenRef<Task>, Task>>("task.get_all_records", new JArray(session), serializer);
        }

        public Pool pool_get_record(string session, string _pool)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<Pool>("pool.get_record", new JArray(session, _pool ?? ""), serializer);
        }

        public XenRef<Pool> pool_get_by_uuid(string session, string _uuid)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Pool>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Pool>>("pool.get_by_uuid", new JArray(session, _uuid ?? ""), serializer);
        }

        public string pool_get_uuid(string session, string _pool)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("pool.get_uuid", new JArray(session, _pool ?? ""), serializer);
        }

        public string pool_get_name_label(string session, string _pool)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("pool.get_name_label", new JArray(session, _pool ?? ""), serializer);
        }

        public string pool_get_name_description(string session, string _pool)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("pool.get_name_description", new JArray(session, _pool ?? ""), serializer);
        }

        public XenRef<Host> pool_get_master(string session, string _pool)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Host>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Host>>("pool.get_master", new JArray(session, _pool ?? ""), serializer);
        }

        public XenRef<SR> pool_get_default_sr(string session, string _pool)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<SR>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<SR>>("pool.get_default_SR", new JArray(session, _pool ?? ""), serializer);
        }

        public XenRef<SR> pool_get_suspend_image_sr(string session, string _pool)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<SR>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<SR>>("pool.get_suspend_image_SR", new JArray(session, _pool ?? ""), serializer);
        }

        public XenRef<SR> pool_get_crash_dump_sr(string session, string _pool)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<SR>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<SR>>("pool.get_crash_dump_SR", new JArray(session, _pool ?? ""), serializer);
        }

        public Dictionary<string, string> pool_get_other_config(string session, string _pool)
        {
            var converters = new List<JsonConverter> {new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<string, string>>("pool.get_other_config", new JArray(session, _pool ?? ""), serializer);
        }

        public bool pool_get_ha_enabled(string session, string _pool)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<bool>("pool.get_ha_enabled", new JArray(session, _pool ?? ""), serializer);
        }

        public Dictionary<string, string> pool_get_ha_configuration(string session, string _pool)
        {
            var converters = new List<JsonConverter> {new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<string, string>>("pool.get_ha_configuration", new JArray(session, _pool ?? ""), serializer);
        }

        public string[] pool_get_ha_statefiles(string session, string _pool)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string[]>("pool.get_ha_statefiles", new JArray(session, _pool ?? ""), serializer);
        }

        public long pool_get_ha_host_failures_to_tolerate(string session, string _pool)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<long>("pool.get_ha_host_failures_to_tolerate", new JArray(session, _pool ?? ""), serializer);
        }

        public long pool_get_ha_plan_exists_for(string session, string _pool)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<long>("pool.get_ha_plan_exists_for", new JArray(session, _pool ?? ""), serializer);
        }

        public bool pool_get_ha_allow_overcommit(string session, string _pool)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<bool>("pool.get_ha_allow_overcommit", new JArray(session, _pool ?? ""), serializer);
        }

        public bool pool_get_ha_overcommitted(string session, string _pool)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<bool>("pool.get_ha_overcommitted", new JArray(session, _pool ?? ""), serializer);
        }

        public Dictionary<string, XenRef<Blob>> pool_get_blobs(string session, string _pool)
        {
            var converters = new List<JsonConverter> {new StringXenRefMapConverter<Blob>()};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<string, XenRef<Blob>>>("pool.get_blobs", new JArray(session, _pool ?? ""), serializer);
        }

        public string[] pool_get_tags(string session, string _pool)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string[]>("pool.get_tags", new JArray(session, _pool ?? ""), serializer);
        }

        public Dictionary<string, string> pool_get_gui_config(string session, string _pool)
        {
            var converters = new List<JsonConverter> {new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<string, string>>("pool.get_gui_config", new JArray(session, _pool ?? ""), serializer);
        }

        public Dictionary<string, string> pool_get_health_check_config(string session, string _pool)
        {
            var converters = new List<JsonConverter> {new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<string, string>>("pool.get_health_check_config", new JArray(session, _pool ?? ""), serializer);
        }

        public string pool_get_wlb_url(string session, string _pool)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("pool.get_wlb_url", new JArray(session, _pool ?? ""), serializer);
        }

        public string pool_get_wlb_username(string session, string _pool)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("pool.get_wlb_username", new JArray(session, _pool ?? ""), serializer);
        }

        public bool pool_get_wlb_enabled(string session, string _pool)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<bool>("pool.get_wlb_enabled", new JArray(session, _pool ?? ""), serializer);
        }

        public bool pool_get_wlb_verify_cert(string session, string _pool)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<bool>("pool.get_wlb_verify_cert", new JArray(session, _pool ?? ""), serializer);
        }

        public bool pool_get_redo_log_enabled(string session, string _pool)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<bool>("pool.get_redo_log_enabled", new JArray(session, _pool ?? ""), serializer);
        }

        public XenRef<VDI> pool_get_redo_log_vdi(string session, string _pool)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<VDI>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<VDI>>("pool.get_redo_log_vdi", new JArray(session, _pool ?? ""), serializer);
        }

        public string pool_get_vswitch_controller(string session, string _pool)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("pool.get_vswitch_controller", new JArray(session, _pool ?? ""), serializer);
        }

        public Dictionary<string, string> pool_get_restrictions(string session, string _pool)
        {
            var converters = new List<JsonConverter> {new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<string, string>>("pool.get_restrictions", new JArray(session, _pool ?? ""), serializer);
        }

        public List<XenRef<VDI>> pool_get_metadata_vdis(string session, string _pool)
        {
            var converters = new List<JsonConverter> {new XenRefListConverter<VDI>()};
            var serializer = CreateSerializer(converters);
            return Rpc<List<XenRef<VDI>>>("pool.get_metadata_VDIs", new JArray(session, _pool ?? ""), serializer);
        }

        public string pool_get_ha_cluster_stack(string session, string _pool)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("pool.get_ha_cluster_stack", new JArray(session, _pool ?? ""), serializer);
        }

        public List<pool_allowed_operations> pool_get_allowed_operations(string session, string _pool)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<List<pool_allowed_operations>>("pool.get_allowed_operations", new JArray(session, _pool ?? ""), serializer);
        }

        public Dictionary<string, pool_allowed_operations> pool_get_current_operations(string session, string _pool)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<string, pool_allowed_operations>>("pool.get_current_operations", new JArray(session, _pool ?? ""), serializer);
        }

        public Dictionary<string, string> pool_get_guest_agent_config(string session, string _pool)
        {
            var converters = new List<JsonConverter> {new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<string, string>>("pool.get_guest_agent_config", new JArray(session, _pool ?? ""), serializer);
        }

        public Dictionary<string, string> pool_get_cpu_info(string session, string _pool)
        {
            var converters = new List<JsonConverter> {new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<string, string>>("pool.get_cpu_info", new JArray(session, _pool ?? ""), serializer);
        }

        public bool pool_get_policy_no_vendor_device(string session, string _pool)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<bool>("pool.get_policy_no_vendor_device", new JArray(session, _pool ?? ""), serializer);
        }

        public bool pool_get_live_patching_disabled(string session, string _pool)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<bool>("pool.get_live_patching_disabled", new JArray(session, _pool ?? ""), serializer);
        }

        public bool pool_get_igmp_snooping_enabled(string session, string _pool)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<bool>("pool.get_igmp_snooping_enabled", new JArray(session, _pool ?? ""), serializer);
        }

        public void pool_set_name_label(string session, string _pool, string _name_label)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("pool.set_name_label", new JArray(session, _pool ?? "", _name_label ?? ""), serializer);
        }

        public void pool_set_name_description(string session, string _pool, string _name_description)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("pool.set_name_description", new JArray(session, _pool ?? "", _name_description ?? ""), serializer);
        }

        public void pool_set_default_sr(string session, string _pool, string _default_sr)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<SR>()};
            var serializer = CreateSerializer(converters);
            Rpc("pool.set_default_SR", new JArray(session, _pool ?? "", _default_sr ?? ""), serializer);
        }

        public void pool_set_suspend_image_sr(string session, string _pool, string _suspend_image_sr)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<SR>()};
            var serializer = CreateSerializer(converters);
            Rpc("pool.set_suspend_image_SR", new JArray(session, _pool ?? "", _suspend_image_sr ?? ""), serializer);
        }

        public void pool_set_crash_dump_sr(string session, string _pool, string _crash_dump_sr)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<SR>()};
            var serializer = CreateSerializer(converters);
            Rpc("pool.set_crash_dump_SR", new JArray(session, _pool ?? "", _crash_dump_sr ?? ""), serializer);
        }

        public void pool_set_other_config(string session, string _pool, Dictionary<string, string> _other_config)
        {
            var converters = new List<JsonConverter> {new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            Rpc("pool.set_other_config", new JArray(session, _pool ?? "", _other_config == null ? new JObject() : JObject.FromObject(_other_config, serializer)), serializer);
        }

        public void pool_add_to_other_config(string session, string _pool, string _key, string _value)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("pool.add_to_other_config", new JArray(session, _pool ?? "", _key ?? "", _value ?? ""), serializer);
        }

        public void pool_remove_from_other_config(string session, string _pool, string _key)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("pool.remove_from_other_config", new JArray(session, _pool ?? "", _key ?? ""), serializer);
        }

        public void pool_set_ha_allow_overcommit(string session, string _pool, bool _ha_allow_overcommit)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("pool.set_ha_allow_overcommit", new JArray(session, _pool ?? "", _ha_allow_overcommit), serializer);
        }

        public void pool_set_tags(string session, string _pool, string[] _tags)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("pool.set_tags", new JArray(session, _pool ?? "", _tags == null ? new JArray() : JArray.FromObject(_tags)), serializer);
        }

        public void pool_add_tags(string session, string _pool, string _value)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("pool.add_tags", new JArray(session, _pool ?? "", _value ?? ""), serializer);
        }

        public void pool_remove_tags(string session, string _pool, string _value)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("pool.remove_tags", new JArray(session, _pool ?? "", _value ?? ""), serializer);
        }

        public void pool_set_gui_config(string session, string _pool, Dictionary<string, string> _gui_config)
        {
            var converters = new List<JsonConverter> {new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            Rpc("pool.set_gui_config", new JArray(session, _pool ?? "", _gui_config == null ? new JObject() : JObject.FromObject(_gui_config, serializer)), serializer);
        }

        public void pool_add_to_gui_config(string session, string _pool, string _key, string _value)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("pool.add_to_gui_config", new JArray(session, _pool ?? "", _key ?? "", _value ?? ""), serializer);
        }

        public void pool_remove_from_gui_config(string session, string _pool, string _key)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("pool.remove_from_gui_config", new JArray(session, _pool ?? "", _key ?? ""), serializer);
        }

        public void pool_set_health_check_config(string session, string _pool, Dictionary<string, string> _health_check_config)
        {
            var converters = new List<JsonConverter> {new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            Rpc("pool.set_health_check_config", new JArray(session, _pool ?? "", _health_check_config == null ? new JObject() : JObject.FromObject(_health_check_config, serializer)), serializer);
        }

        public void pool_add_to_health_check_config(string session, string _pool, string _key, string _value)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("pool.add_to_health_check_config", new JArray(session, _pool ?? "", _key ?? "", _value ?? ""), serializer);
        }

        public void pool_remove_from_health_check_config(string session, string _pool, string _key)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("pool.remove_from_health_check_config", new JArray(session, _pool ?? "", _key ?? ""), serializer);
        }

        public void pool_set_wlb_enabled(string session, string _pool, bool _wlb_enabled)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("pool.set_wlb_enabled", new JArray(session, _pool ?? "", _wlb_enabled), serializer);
        }

        public void pool_set_wlb_verify_cert(string session, string _pool, bool _wlb_verify_cert)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("pool.set_wlb_verify_cert", new JArray(session, _pool ?? "", _wlb_verify_cert), serializer);
        }

        public void pool_set_policy_no_vendor_device(string session, string _pool, bool _policy_no_vendor_device)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("pool.set_policy_no_vendor_device", new JArray(session, _pool ?? "", _policy_no_vendor_device), serializer);
        }

        public void pool_set_live_patching_disabled(string session, string _pool, bool _live_patching_disabled)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("pool.set_live_patching_disabled", new JArray(session, _pool ?? "", _live_patching_disabled), serializer);
        }

        public void pool_join(string session, string _master_address, string _master_username, string _master_password)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("pool.join", new JArray(session, _master_address ?? "", _master_username ?? "", _master_password ?? ""), serializer);
        }

        public XenRef<Task> async_pool_join(string session, string _master_address, string _master_username, string _master_password)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.pool.join", new JArray(session, _master_address ?? "", _master_username ?? "", _master_password ?? ""), serializer);
        }

        public void pool_join_force(string session, string _master_address, string _master_username, string _master_password)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("pool.join_force", new JArray(session, _master_address ?? "", _master_username ?? "", _master_password ?? ""), serializer);
        }

        public XenRef<Task> async_pool_join_force(string session, string _master_address, string _master_username, string _master_password)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.pool.join_force", new JArray(session, _master_address ?? "", _master_username ?? "", _master_password ?? ""), serializer);
        }

        public void pool_eject(string session, string _host)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Host>()};
            var serializer = CreateSerializer(converters);
            Rpc("pool.eject", new JArray(session, _host ?? ""), serializer);
        }

        public XenRef<Task> async_pool_eject(string session, string _host)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>(), new XenRefConverter<Host>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.pool.eject", new JArray(session, _host ?? ""), serializer);
        }

        public void pool_emergency_transition_to_master(string session)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("pool.emergency_transition_to_master", new JArray(session), serializer);
        }

        public void pool_emergency_reset_master(string session, string _master_address)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("pool.emergency_reset_master", new JArray(session, _master_address ?? ""), serializer);
        }

        public List<XenRef<Host>> pool_recover_slaves(string session)
        {
            var converters = new List<JsonConverter> {new XenRefListConverter<Host>()};
            var serializer = CreateSerializer(converters);
            return Rpc<List<XenRef<Host>>>("pool.recover_slaves", new JArray(session), serializer);
        }

        public XenRef<Task> async_pool_recover_slaves(string session)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.pool.recover_slaves", new JArray(session), serializer);
        }

        public List<XenRef<PIF>> pool_create_vlan(string session, string _device, string _network, long _vlan)
        {
            var converters = new List<JsonConverter> {new XenRefListConverter<PIF>(), new XenRefConverter<Network>()};
            var serializer = CreateSerializer(converters);
            return Rpc<List<XenRef<PIF>>>("pool.create_VLAN", new JArray(session, _device ?? "", _network ?? "", _vlan), serializer);
        }

        public XenRef<Task> async_pool_create_vlan(string session, string _device, string _network, long _vlan)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>(), new XenRefConverter<Network>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.pool.create_VLAN", new JArray(session, _device ?? "", _network ?? "", _vlan), serializer);
        }

        public void pool_management_reconfigure(string session, string _network)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Network>()};
            var serializer = CreateSerializer(converters);
            Rpc("pool.management_reconfigure", new JArray(session, _network ?? ""), serializer);
        }

        public XenRef<Task> async_pool_management_reconfigure(string session, string _network)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>(), new XenRefConverter<Network>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.pool.management_reconfigure", new JArray(session, _network ?? ""), serializer);
        }

        public List<XenRef<PIF>> pool_create_vlan_from_pif(string session, string _pif, string _network, long _vlan)
        {
            var converters = new List<JsonConverter> {new XenRefListConverter<PIF>(), new XenRefConverter<PIF>(), new XenRefConverter<Network>()};
            var serializer = CreateSerializer(converters);
            return Rpc<List<XenRef<PIF>>>("pool.create_VLAN_from_PIF", new JArray(session, _pif ?? "", _network ?? "", _vlan), serializer);
        }

        public XenRef<Task> async_pool_create_vlan_from_pif(string session, string _pif, string _network, long _vlan)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>(), new XenRefConverter<PIF>(), new XenRefConverter<Network>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.pool.create_VLAN_from_PIF", new JArray(session, _pif ?? "", _network ?? "", _vlan), serializer);
        }

        public void pool_enable_ha(string session, List<XenRef<SR>> _heartbeat_srs, Dictionary<string, string> _configuration)
        {
            var converters = new List<JsonConverter> {new XenRefListConverter<SR>(), new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            Rpc("pool.enable_ha", new JArray(session, _heartbeat_srs == null ? new JArray() : JArray.FromObject(_heartbeat_srs, serializer), _configuration == null ? new JObject() : JObject.FromObject(_configuration, serializer)), serializer);
        }

        public XenRef<Task> async_pool_enable_ha(string session, List<XenRef<SR>> _heartbeat_srs, Dictionary<string, string> _configuration)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>(), new XenRefListConverter<SR>(), new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.pool.enable_ha", new JArray(session, _heartbeat_srs == null ? new JArray() : JArray.FromObject(_heartbeat_srs, serializer), _configuration == null ? new JObject() : JObject.FromObject(_configuration, serializer)), serializer);
        }

        public void pool_disable_ha(string session)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("pool.disable_ha", new JArray(session), serializer);
        }

        public XenRef<Task> async_pool_disable_ha(string session)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.pool.disable_ha", new JArray(session), serializer);
        }

        public void pool_sync_database(string session)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("pool.sync_database", new JArray(session), serializer);
        }

        public XenRef<Task> async_pool_sync_database(string session)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.pool.sync_database", new JArray(session), serializer);
        }

        public void pool_designate_new_master(string session, string _host)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Host>()};
            var serializer = CreateSerializer(converters);
            Rpc("pool.designate_new_master", new JArray(session, _host ?? ""), serializer);
        }

        public XenRef<Task> async_pool_designate_new_master(string session, string _host)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>(), new XenRefConverter<Host>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.pool.designate_new_master", new JArray(session, _host ?? ""), serializer);
        }

        public void pool_ha_prevent_restarts_for(string session, long _seconds)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("pool.ha_prevent_restarts_for", new JArray(session, _seconds), serializer);
        }

        public bool pool_ha_failover_plan_exists(string session, long _n)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<bool>("pool.ha_failover_plan_exists", new JArray(session, _n), serializer);
        }

        public long pool_ha_compute_max_host_failures_to_tolerate(string session)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<long>("pool.ha_compute_max_host_failures_to_tolerate", new JArray(session), serializer);
        }

        public long pool_ha_compute_hypothetical_max_host_failures_to_tolerate(string session, Dictionary<XenRef<VM>, string> _configuration)
        {
            var converters = new List<JsonConverter> {new XenRefStringMapConverter<VM>()};
            var serializer = CreateSerializer(converters);
            return Rpc<long>("pool.ha_compute_hypothetical_max_host_failures_to_tolerate", new JArray(session, _configuration == null ? new JObject() : JObject.FromObject(_configuration, serializer)), serializer);
        }

        public Dictionary<XenRef<VM>, Dictionary<string, string>> pool_ha_compute_vm_failover_plan(string session, List<XenRef<Host>> _failed_hosts, List<XenRef<VM>> _failed_vms)
        {
            var converters = new List<JsonConverter> {new XenRefStringStringMapMapConverter<VM>(), new XenRefListConverter<Host>(), new XenRefListConverter<VM>()};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<XenRef<VM>, Dictionary<string, string>>>("pool.ha_compute_vm_failover_plan", new JArray(session, _failed_hosts == null ? new JArray() : JArray.FromObject(_failed_hosts, serializer), _failed_vms == null ? new JArray() : JArray.FromObject(_failed_vms, serializer)), serializer);
        }

        public void pool_set_ha_host_failures_to_tolerate(string session, string _pool, long _value)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("pool.set_ha_host_failures_to_tolerate", new JArray(session, _pool ?? "", _value), serializer);
        }

        public XenRef<Task> async_pool_set_ha_host_failures_to_tolerate(string session, string _pool, long _value)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.pool.set_ha_host_failures_to_tolerate", new JArray(session, _pool ?? "", _value), serializer);
        }

        public XenRef<Blob> pool_create_new_blob(string session, string _pool, string _name, string _mime_type)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Blob>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Blob>>("pool.create_new_blob", new JArray(session, _pool ?? "", _name ?? "", _mime_type ?? ""), serializer);
        }

        public XenRef<Task> async_pool_create_new_blob(string session, string _pool, string _name, string _mime_type)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.pool.create_new_blob", new JArray(session, _pool ?? "", _name ?? "", _mime_type ?? ""), serializer);
        }

        public XenRef<Blob> pool_create_new_blob(string session, string _pool, string _name, string _mime_type, bool _public)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Blob>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Blob>>("pool.create_new_blob", new JArray(session, _pool ?? "", _name ?? "", _mime_type ?? "", _public), serializer);
        }

        public XenRef<Task> async_pool_create_new_blob(string session, string _pool, string _name, string _mime_type, bool _public)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.pool.create_new_blob", new JArray(session, _pool ?? "", _name ?? "", _mime_type ?? "", _public), serializer);
        }

        public void pool_enable_external_auth(string session, string _pool, Dictionary<string, string> _config, string _service_name, string _auth_type)
        {
            var converters = new List<JsonConverter> {new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            Rpc("pool.enable_external_auth", new JArray(session, _pool ?? "", _config == null ? new JObject() : JObject.FromObject(_config, serializer), _service_name ?? "", _auth_type ?? ""), serializer);
        }

        public void pool_disable_external_auth(string session, string _pool, Dictionary<string, string> _config)
        {
            var converters = new List<JsonConverter> {new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            Rpc("pool.disable_external_auth", new JArray(session, _pool ?? "", _config == null ? new JObject() : JObject.FromObject(_config, serializer)), serializer);
        }

        public void pool_detect_nonhomogeneous_external_auth(string session, string _pool)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("pool.detect_nonhomogeneous_external_auth", new JArray(session, _pool ?? ""), serializer);
        }

        public void pool_initialize_wlb(string session, string _wlb_url, string _wlb_username, string _wlb_password, string _xenserver_username, string _xenserver_password)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("pool.initialize_wlb", new JArray(session, _wlb_url ?? "", _wlb_username ?? "", _wlb_password ?? "", _xenserver_username ?? "", _xenserver_password ?? ""), serializer);
        }

        public XenRef<Task> async_pool_initialize_wlb(string session, string _wlb_url, string _wlb_username, string _wlb_password, string _xenserver_username, string _xenserver_password)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.pool.initialize_wlb", new JArray(session, _wlb_url ?? "", _wlb_username ?? "", _wlb_password ?? "", _xenserver_username ?? "", _xenserver_password ?? ""), serializer);
        }

        public void pool_deconfigure_wlb(string session)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("pool.deconfigure_wlb", new JArray(session), serializer);
        }

        public XenRef<Task> async_pool_deconfigure_wlb(string session)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.pool.deconfigure_wlb", new JArray(session), serializer);
        }

        public void pool_send_wlb_configuration(string session, Dictionary<string, string> _config)
        {
            var converters = new List<JsonConverter> {new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            Rpc("pool.send_wlb_configuration", new JArray(session, _config == null ? new JObject() : JObject.FromObject(_config, serializer)), serializer);
        }

        public XenRef<Task> async_pool_send_wlb_configuration(string session, Dictionary<string, string> _config)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>(), new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.pool.send_wlb_configuration", new JArray(session, _config == null ? new JObject() : JObject.FromObject(_config, serializer)), serializer);
        }

        public Dictionary<string, string> pool_retrieve_wlb_configuration(string session)
        {
            var converters = new List<JsonConverter> {new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<string, string>>("pool.retrieve_wlb_configuration", new JArray(session), serializer);
        }

        public XenRef<Task> async_pool_retrieve_wlb_configuration(string session)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.pool.retrieve_wlb_configuration", new JArray(session), serializer);
        }

        public Dictionary<XenRef<VM>, string[]> pool_retrieve_wlb_recommendations(string session)
        {
            var converters = new List<JsonConverter> {new XenRefStringSetMapConverter<VM>()};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<XenRef<VM>, string[]>>("pool.retrieve_wlb_recommendations", new JArray(session), serializer);
        }

        public XenRef<Task> async_pool_retrieve_wlb_recommendations(string session)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.pool.retrieve_wlb_recommendations", new JArray(session), serializer);
        }

        public string pool_send_test_post(string session, string _host, long _port, string _body)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("pool.send_test_post", new JArray(session, _host ?? "", _port, _body ?? ""), serializer);
        }

        public XenRef<Task> async_pool_send_test_post(string session, string _host, long _port, string _body)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.pool.send_test_post", new JArray(session, _host ?? "", _port, _body ?? ""), serializer);
        }

        public void pool_certificate_install(string session, string _name, string _cert)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("pool.certificate_install", new JArray(session, _name ?? "", _cert ?? ""), serializer);
        }

        public XenRef<Task> async_pool_certificate_install(string session, string _name, string _cert)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.pool.certificate_install", new JArray(session, _name ?? "", _cert ?? ""), serializer);
        }

        public void pool_certificate_uninstall(string session, string _name)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("pool.certificate_uninstall", new JArray(session, _name ?? ""), serializer);
        }

        public XenRef<Task> async_pool_certificate_uninstall(string session, string _name)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.pool.certificate_uninstall", new JArray(session, _name ?? ""), serializer);
        }

        public string[] pool_certificate_list(string session)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string[]>("pool.certificate_list", new JArray(session), serializer);
        }

        public XenRef<Task> async_pool_certificate_list(string session)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.pool.certificate_list", new JArray(session), serializer);
        }

        public void pool_crl_install(string session, string _name, string _cert)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("pool.crl_install", new JArray(session, _name ?? "", _cert ?? ""), serializer);
        }

        public XenRef<Task> async_pool_crl_install(string session, string _name, string _cert)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.pool.crl_install", new JArray(session, _name ?? "", _cert ?? ""), serializer);
        }

        public void pool_crl_uninstall(string session, string _name)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("pool.crl_uninstall", new JArray(session, _name ?? ""), serializer);
        }

        public XenRef<Task> async_pool_crl_uninstall(string session, string _name)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.pool.crl_uninstall", new JArray(session, _name ?? ""), serializer);
        }

        public string[] pool_crl_list(string session)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string[]>("pool.crl_list", new JArray(session), serializer);
        }

        public XenRef<Task> async_pool_crl_list(string session)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.pool.crl_list", new JArray(session), serializer);
        }

        public void pool_certificate_sync(string session)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("pool.certificate_sync", new JArray(session), serializer);
        }

        public XenRef<Task> async_pool_certificate_sync(string session)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.pool.certificate_sync", new JArray(session), serializer);
        }

        public void pool_enable_redo_log(string session, string _sr)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<SR>()};
            var serializer = CreateSerializer(converters);
            Rpc("pool.enable_redo_log", new JArray(session, _sr ?? ""), serializer);
        }

        public XenRef<Task> async_pool_enable_redo_log(string session, string _sr)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>(), new XenRefConverter<SR>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.pool.enable_redo_log", new JArray(session, _sr ?? ""), serializer);
        }

        public void pool_disable_redo_log(string session)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("pool.disable_redo_log", new JArray(session), serializer);
        }

        public XenRef<Task> async_pool_disable_redo_log(string session)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.pool.disable_redo_log", new JArray(session), serializer);
        }

        public void pool_set_vswitch_controller(string session, string _address)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("pool.set_vswitch_controller", new JArray(session, _address ?? ""), serializer);
        }

        public XenRef<Task> async_pool_set_vswitch_controller(string session, string _address)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.pool.set_vswitch_controller", new JArray(session, _address ?? ""), serializer);
        }

        public string pool_test_archive_target(string session, string _pool, Dictionary<string, string> _config)
        {
            var converters = new List<JsonConverter> {new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("pool.test_archive_target", new JArray(session, _pool ?? "", _config == null ? new JObject() : JObject.FromObject(_config, serializer)), serializer);
        }

        public void pool_enable_local_storage_caching(string session, string _pool)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("pool.enable_local_storage_caching", new JArray(session, _pool ?? ""), serializer);
        }

        public XenRef<Task> async_pool_enable_local_storage_caching(string session, string _pool)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.pool.enable_local_storage_caching", new JArray(session, _pool ?? ""), serializer);
        }

        public void pool_disable_local_storage_caching(string session, string _pool)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("pool.disable_local_storage_caching", new JArray(session, _pool ?? ""), serializer);
        }

        public XenRef<Task> async_pool_disable_local_storage_caching(string session, string _pool)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.pool.disable_local_storage_caching", new JArray(session, _pool ?? ""), serializer);
        }

        public Dictionary<string, string> pool_get_license_state(string session, string _pool)
        {
            var converters = new List<JsonConverter> {new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<string, string>>("pool.get_license_state", new JArray(session, _pool ?? ""), serializer);
        }

        public XenRef<Task> async_pool_get_license_state(string session, string _pool)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.pool.get_license_state", new JArray(session, _pool ?? ""), serializer);
        }

        public void pool_apply_edition(string session, string _pool, string _edition)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("pool.apply_edition", new JArray(session, _pool ?? "", _edition ?? ""), serializer);
        }

        public XenRef<Task> async_pool_apply_edition(string session, string _pool, string _edition)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.pool.apply_edition", new JArray(session, _pool ?? "", _edition ?? ""), serializer);
        }

        public void pool_enable_ssl_legacy(string session, string _pool)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("pool.enable_ssl_legacy", new JArray(session, _pool ?? ""), serializer);
        }

        public XenRef<Task> async_pool_enable_ssl_legacy(string session, string _pool)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.pool.enable_ssl_legacy", new JArray(session, _pool ?? ""), serializer);
        }

        public void pool_disable_ssl_legacy(string session, string _pool)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("pool.disable_ssl_legacy", new JArray(session, _pool ?? ""), serializer);
        }

        public XenRef<Task> async_pool_disable_ssl_legacy(string session, string _pool)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.pool.disable_ssl_legacy", new JArray(session, _pool ?? ""), serializer);
        }

        public void pool_set_igmp_snooping_enabled(string session, string _pool, bool _value)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("pool.set_igmp_snooping_enabled", new JArray(session, _pool ?? "", _value), serializer);
        }

        public XenRef<Task> async_pool_set_igmp_snooping_enabled(string session, string _pool, bool _value)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.pool.set_igmp_snooping_enabled", new JArray(session, _pool ?? "", _value), serializer);
        }

        public bool pool_has_extension(string session, string _pool, string _name)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<bool>("pool.has_extension", new JArray(session, _pool ?? "", _name ?? ""), serializer);
        }

        public XenRef<Task> async_pool_has_extension(string session, string _pool, string _name)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.pool.has_extension", new JArray(session, _pool ?? "", _name ?? ""), serializer);
        }

        public void pool_add_to_guest_agent_config(string session, string _pool, string _key, string _value)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("pool.add_to_guest_agent_config", new JArray(session, _pool ?? "", _key ?? "", _value ?? ""), serializer);
        }

        public XenRef<Task> async_pool_add_to_guest_agent_config(string session, string _pool, string _key, string _value)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.pool.add_to_guest_agent_config", new JArray(session, _pool ?? "", _key ?? "", _value ?? ""), serializer);
        }

        public void pool_remove_from_guest_agent_config(string session, string _pool, string _key)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("pool.remove_from_guest_agent_config", new JArray(session, _pool ?? "", _key ?? ""), serializer);
        }

        public XenRef<Task> async_pool_remove_from_guest_agent_config(string session, string _pool, string _key)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.pool.remove_from_guest_agent_config", new JArray(session, _pool ?? "", _key ?? ""), serializer);
        }

        public List<XenRef<Pool>> pool_get_all(string session)
        {
            var converters = new List<JsonConverter> {new XenRefListConverter<Pool>()};
            var serializer = CreateSerializer(converters);
            return Rpc<List<XenRef<Pool>>>("pool.get_all", new JArray(session), serializer);
        }

        public Dictionary<XenRef<Pool>, Pool> pool_get_all_records(string session)
        {
            var converters = new List<JsonConverter> {new XenRefXenObjectMapConverter<Pool>()};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<XenRef<Pool>, Pool>>("pool.get_all_records", new JArray(session), serializer);
        }

        public Pool_patch pool_patch_get_record(string session, string _pool_patch)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<Pool_patch>("pool_patch.get_record", new JArray(session, _pool_patch ?? ""), serializer);
        }

        public XenRef<Pool_patch> pool_patch_get_by_uuid(string session, string _uuid)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Pool_patch>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Pool_patch>>("pool_patch.get_by_uuid", new JArray(session, _uuid ?? ""), serializer);
        }

        public List<XenRef<Pool_patch>> pool_patch_get_by_name_label(string session, string _label)
        {
            var converters = new List<JsonConverter> {new XenRefListConverter<Pool_patch>()};
            var serializer = CreateSerializer(converters);
            return Rpc<List<XenRef<Pool_patch>>>("pool_patch.get_by_name_label", new JArray(session, _label ?? ""), serializer);
        }

        public string pool_patch_get_uuid(string session, string _pool_patch)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("pool_patch.get_uuid", new JArray(session, _pool_patch ?? ""), serializer);
        }

        public string pool_patch_get_name_label(string session, string _pool_patch)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("pool_patch.get_name_label", new JArray(session, _pool_patch ?? ""), serializer);
        }

        public string pool_patch_get_name_description(string session, string _pool_patch)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("pool_patch.get_name_description", new JArray(session, _pool_patch ?? ""), serializer);
        }

        public string pool_patch_get_version(string session, string _pool_patch)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("pool_patch.get_version", new JArray(session, _pool_patch ?? ""), serializer);
        }

        public long pool_patch_get_size(string session, string _pool_patch)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<long>("pool_patch.get_size", new JArray(session, _pool_patch ?? ""), serializer);
        }

        public bool pool_patch_get_pool_applied(string session, string _pool_patch)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<bool>("pool_patch.get_pool_applied", new JArray(session, _pool_patch ?? ""), serializer);
        }

        public List<XenRef<Host_patch>> pool_patch_get_host_patches(string session, string _pool_patch)
        {
            var converters = new List<JsonConverter> {new XenRefListConverter<Host_patch>()};
            var serializer = CreateSerializer(converters);
            return Rpc<List<XenRef<Host_patch>>>("pool_patch.get_host_patches", new JArray(session, _pool_patch ?? ""), serializer);
        }

        public List<after_apply_guidance> pool_patch_get_after_apply_guidance(string session, string _pool_patch)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<List<after_apply_guidance>>("pool_patch.get_after_apply_guidance", new JArray(session, _pool_patch ?? ""), serializer);
        }

        public XenRef<Pool_update> pool_patch_get_pool_update(string session, string _pool_patch)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Pool_update>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Pool_update>>("pool_patch.get_pool_update", new JArray(session, _pool_patch ?? ""), serializer);
        }

        public Dictionary<string, string> pool_patch_get_other_config(string session, string _pool_patch)
        {
            var converters = new List<JsonConverter> {new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<string, string>>("pool_patch.get_other_config", new JArray(session, _pool_patch ?? ""), serializer);
        }

        public void pool_patch_set_other_config(string session, string _pool_patch, Dictionary<string, string> _other_config)
        {
            var converters = new List<JsonConverter> {new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            Rpc("pool_patch.set_other_config", new JArray(session, _pool_patch ?? "", _other_config == null ? new JObject() : JObject.FromObject(_other_config, serializer)), serializer);
        }

        public void pool_patch_add_to_other_config(string session, string _pool_patch, string _key, string _value)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("pool_patch.add_to_other_config", new JArray(session, _pool_patch ?? "", _key ?? "", _value ?? ""), serializer);
        }

        public void pool_patch_remove_from_other_config(string session, string _pool_patch, string _key)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("pool_patch.remove_from_other_config", new JArray(session, _pool_patch ?? "", _key ?? ""), serializer);
        }

        public string pool_patch_apply(string session, string _pool_patch, string _host)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Host>()};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("pool_patch.apply", new JArray(session, _pool_patch ?? "", _host ?? ""), serializer);
        }

        public XenRef<Task> async_pool_patch_apply(string session, string _pool_patch, string _host)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>(), new XenRefConverter<Host>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.pool_patch.apply", new JArray(session, _pool_patch ?? "", _host ?? ""), serializer);
        }

        public void pool_patch_pool_apply(string session, string _pool_patch)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("pool_patch.pool_apply", new JArray(session, _pool_patch ?? ""), serializer);
        }

        public XenRef<Task> async_pool_patch_pool_apply(string session, string _pool_patch)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.pool_patch.pool_apply", new JArray(session, _pool_patch ?? ""), serializer);
        }

        public string pool_patch_precheck(string session, string _pool_patch, string _host)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Host>()};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("pool_patch.precheck", new JArray(session, _pool_patch ?? "", _host ?? ""), serializer);
        }

        public XenRef<Task> async_pool_patch_precheck(string session, string _pool_patch, string _host)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>(), new XenRefConverter<Host>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.pool_patch.precheck", new JArray(session, _pool_patch ?? "", _host ?? ""), serializer);
        }

        public void pool_patch_clean(string session, string _pool_patch)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("pool_patch.clean", new JArray(session, _pool_patch ?? ""), serializer);
        }

        public XenRef<Task> async_pool_patch_clean(string session, string _pool_patch)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.pool_patch.clean", new JArray(session, _pool_patch ?? ""), serializer);
        }

        public void pool_patch_pool_clean(string session, string _pool_patch)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("pool_patch.pool_clean", new JArray(session, _pool_patch ?? ""), serializer);
        }

        public XenRef<Task> async_pool_patch_pool_clean(string session, string _pool_patch)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.pool_patch.pool_clean", new JArray(session, _pool_patch ?? ""), serializer);
        }

        public void pool_patch_destroy(string session, string _pool_patch)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("pool_patch.destroy", new JArray(session, _pool_patch ?? ""), serializer);
        }

        public XenRef<Task> async_pool_patch_destroy(string session, string _pool_patch)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.pool_patch.destroy", new JArray(session, _pool_patch ?? ""), serializer);
        }

        public void pool_patch_clean_on_host(string session, string _pool_patch, string _host)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Host>()};
            var serializer = CreateSerializer(converters);
            Rpc("pool_patch.clean_on_host", new JArray(session, _pool_patch ?? "", _host ?? ""), serializer);
        }

        public XenRef<Task> async_pool_patch_clean_on_host(string session, string _pool_patch, string _host)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>(), new XenRefConverter<Host>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.pool_patch.clean_on_host", new JArray(session, _pool_patch ?? "", _host ?? ""), serializer);
        }

        public List<XenRef<Pool_patch>> pool_patch_get_all(string session)
        {
            var converters = new List<JsonConverter> {new XenRefListConverter<Pool_patch>()};
            var serializer = CreateSerializer(converters);
            return Rpc<List<XenRef<Pool_patch>>>("pool_patch.get_all", new JArray(session), serializer);
        }

        public Dictionary<XenRef<Pool_patch>, Pool_patch> pool_patch_get_all_records(string session)
        {
            var converters = new List<JsonConverter> {new XenRefXenObjectMapConverter<Pool_patch>()};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<XenRef<Pool_patch>, Pool_patch>>("pool_patch.get_all_records", new JArray(session), serializer);
        }

        public Pool_update pool_update_get_record(string session, string _pool_update)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<Pool_update>("pool_update.get_record", new JArray(session, _pool_update ?? ""), serializer);
        }

        public XenRef<Pool_update> pool_update_get_by_uuid(string session, string _uuid)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Pool_update>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Pool_update>>("pool_update.get_by_uuid", new JArray(session, _uuid ?? ""), serializer);
        }

        public List<XenRef<Pool_update>> pool_update_get_by_name_label(string session, string _label)
        {
            var converters = new List<JsonConverter> {new XenRefListConverter<Pool_update>()};
            var serializer = CreateSerializer(converters);
            return Rpc<List<XenRef<Pool_update>>>("pool_update.get_by_name_label", new JArray(session, _label ?? ""), serializer);
        }

        public string pool_update_get_uuid(string session, string _pool_update)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("pool_update.get_uuid", new JArray(session, _pool_update ?? ""), serializer);
        }

        public string pool_update_get_name_label(string session, string _pool_update)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("pool_update.get_name_label", new JArray(session, _pool_update ?? ""), serializer);
        }

        public string pool_update_get_name_description(string session, string _pool_update)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("pool_update.get_name_description", new JArray(session, _pool_update ?? ""), serializer);
        }

        public string pool_update_get_version(string session, string _pool_update)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("pool_update.get_version", new JArray(session, _pool_update ?? ""), serializer);
        }

        public long pool_update_get_installation_size(string session, string _pool_update)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<long>("pool_update.get_installation_size", new JArray(session, _pool_update ?? ""), serializer);
        }

        public string pool_update_get_key(string session, string _pool_update)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("pool_update.get_key", new JArray(session, _pool_update ?? ""), serializer);
        }

        public List<update_after_apply_guidance> pool_update_get_after_apply_guidance(string session, string _pool_update)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<List<update_after_apply_guidance>>("pool_update.get_after_apply_guidance", new JArray(session, _pool_update ?? ""), serializer);
        }

        public XenRef<VDI> pool_update_get_vdi(string session, string _pool_update)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<VDI>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<VDI>>("pool_update.get_vdi", new JArray(session, _pool_update ?? ""), serializer);
        }

        public List<XenRef<Host>> pool_update_get_hosts(string session, string _pool_update)
        {
            var converters = new List<JsonConverter> {new XenRefListConverter<Host>()};
            var serializer = CreateSerializer(converters);
            return Rpc<List<XenRef<Host>>>("pool_update.get_hosts", new JArray(session, _pool_update ?? ""), serializer);
        }

        public Dictionary<string, string> pool_update_get_other_config(string session, string _pool_update)
        {
            var converters = new List<JsonConverter> {new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<string, string>>("pool_update.get_other_config", new JArray(session, _pool_update ?? ""), serializer);
        }

        public bool pool_update_get_enforce_homogeneity(string session, string _pool_update)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<bool>("pool_update.get_enforce_homogeneity", new JArray(session, _pool_update ?? ""), serializer);
        }

        public void pool_update_set_other_config(string session, string _pool_update, Dictionary<string, string> _other_config)
        {
            var converters = new List<JsonConverter> {new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            Rpc("pool_update.set_other_config", new JArray(session, _pool_update ?? "", _other_config == null ? new JObject() : JObject.FromObject(_other_config, serializer)), serializer);
        }

        public void pool_update_add_to_other_config(string session, string _pool_update, string _key, string _value)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("pool_update.add_to_other_config", new JArray(session, _pool_update ?? "", _key ?? "", _value ?? ""), serializer);
        }

        public void pool_update_remove_from_other_config(string session, string _pool_update, string _key)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("pool_update.remove_from_other_config", new JArray(session, _pool_update ?? "", _key ?? ""), serializer);
        }

        public XenRef<Pool_update> pool_update_introduce(string session, string _vdi)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Pool_update>(), new XenRefConverter<VDI>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Pool_update>>("pool_update.introduce", new JArray(session, _vdi ?? ""), serializer);
        }

        public XenRef<Task> async_pool_update_introduce(string session, string _vdi)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>(), new XenRefConverter<VDI>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.pool_update.introduce", new JArray(session, _vdi ?? ""), serializer);
        }

        public livepatch_status pool_update_precheck(string session, string _pool_update, string _host)
        {
            var converters = new List<JsonConverter> {new livepatch_statusConverter(), new XenRefConverter<Host>()};
            var serializer = CreateSerializer(converters);
            return Rpc<livepatch_status>("pool_update.precheck", new JArray(session, _pool_update ?? "", _host ?? ""), serializer);
        }

        public XenRef<Task> async_pool_update_precheck(string session, string _pool_update, string _host)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>(), new XenRefConverter<Host>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.pool_update.precheck", new JArray(session, _pool_update ?? "", _host ?? ""), serializer);
        }

        public void pool_update_apply(string session, string _pool_update, string _host)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Host>()};
            var serializer = CreateSerializer(converters);
            Rpc("pool_update.apply", new JArray(session, _pool_update ?? "", _host ?? ""), serializer);
        }

        public XenRef<Task> async_pool_update_apply(string session, string _pool_update, string _host)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>(), new XenRefConverter<Host>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.pool_update.apply", new JArray(session, _pool_update ?? "", _host ?? ""), serializer);
        }

        public void pool_update_pool_apply(string session, string _pool_update)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("pool_update.pool_apply", new JArray(session, _pool_update ?? ""), serializer);
        }

        public XenRef<Task> async_pool_update_pool_apply(string session, string _pool_update)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.pool_update.pool_apply", new JArray(session, _pool_update ?? ""), serializer);
        }

        public void pool_update_pool_clean(string session, string _pool_update)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("pool_update.pool_clean", new JArray(session, _pool_update ?? ""), serializer);
        }

        public XenRef<Task> async_pool_update_pool_clean(string session, string _pool_update)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.pool_update.pool_clean", new JArray(session, _pool_update ?? ""), serializer);
        }

        public void pool_update_destroy(string session, string _pool_update)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("pool_update.destroy", new JArray(session, _pool_update ?? ""), serializer);
        }

        public XenRef<Task> async_pool_update_destroy(string session, string _pool_update)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.pool_update.destroy", new JArray(session, _pool_update ?? ""), serializer);
        }

        public List<XenRef<Pool_update>> pool_update_get_all(string session)
        {
            var converters = new List<JsonConverter> {new XenRefListConverter<Pool_update>()};
            var serializer = CreateSerializer(converters);
            return Rpc<List<XenRef<Pool_update>>>("pool_update.get_all", new JArray(session), serializer);
        }

        public Dictionary<XenRef<Pool_update>, Pool_update> pool_update_get_all_records(string session)
        {
            var converters = new List<JsonConverter> {new XenRefXenObjectMapConverter<Pool_update>()};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<XenRef<Pool_update>, Pool_update>>("pool_update.get_all_records", new JArray(session), serializer);
        }

        public VM vm_get_record(string session, string _vm)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<VM>("VM.get_record", new JArray(session, _vm ?? ""), serializer);
        }

        public XenRef<VM> vm_get_by_uuid(string session, string _uuid)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<VM>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<VM>>("VM.get_by_uuid", new JArray(session, _uuid ?? ""), serializer);
        }

        public XenRef<VM> vm_create(string session, VM _record)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<VM>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<VM>>("VM.create", new JArray(session, _record.ToJObject()), serializer);
        }

        public XenRef<Task> async_vm_create(string session, VM _record)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.VM.create", new JArray(session, _record.ToJObject()), serializer);
        }

        public void vm_destroy(string session, string _vm)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("VM.destroy", new JArray(session, _vm ?? ""), serializer);
        }

        public XenRef<Task> async_vm_destroy(string session, string _vm)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.VM.destroy", new JArray(session, _vm ?? ""), serializer);
        }

        public List<XenRef<VM>> vm_get_by_name_label(string session, string _label)
        {
            var converters = new List<JsonConverter> {new XenRefListConverter<VM>()};
            var serializer = CreateSerializer(converters);
            return Rpc<List<XenRef<VM>>>("VM.get_by_name_label", new JArray(session, _label ?? ""), serializer);
        }

        public string vm_get_uuid(string session, string _vm)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("VM.get_uuid", new JArray(session, _vm ?? ""), serializer);
        }

        public List<vm_operations> vm_get_allowed_operations(string session, string _vm)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<List<vm_operations>>("VM.get_allowed_operations", new JArray(session, _vm ?? ""), serializer);
        }

        public Dictionary<string, vm_operations> vm_get_current_operations(string session, string _vm)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<string, vm_operations>>("VM.get_current_operations", new JArray(session, _vm ?? ""), serializer);
        }

        public vm_power_state vm_get_power_state(string session, string _vm)
        {
            var converters = new List<JsonConverter> {new vm_power_stateConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<vm_power_state>("VM.get_power_state", new JArray(session, _vm ?? ""), serializer);
        }

        public string vm_get_name_label(string session, string _vm)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("VM.get_name_label", new JArray(session, _vm ?? ""), serializer);
        }

        public string vm_get_name_description(string session, string _vm)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("VM.get_name_description", new JArray(session, _vm ?? ""), serializer);
        }

        public long vm_get_user_version(string session, string _vm)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<long>("VM.get_user_version", new JArray(session, _vm ?? ""), serializer);
        }

        public bool vm_get_is_a_template(string session, string _vm)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<bool>("VM.get_is_a_template", new JArray(session, _vm ?? ""), serializer);
        }

        public bool vm_get_is_default_template(string session, string _vm)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<bool>("VM.get_is_default_template", new JArray(session, _vm ?? ""), serializer);
        }

        public XenRef<VDI> vm_get_suspend_vdi(string session, string _vm)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<VDI>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<VDI>>("VM.get_suspend_VDI", new JArray(session, _vm ?? ""), serializer);
        }

        public XenRef<Host> vm_get_resident_on(string session, string _vm)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Host>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Host>>("VM.get_resident_on", new JArray(session, _vm ?? ""), serializer);
        }

        public XenRef<Host> vm_get_affinity(string session, string _vm)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Host>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Host>>("VM.get_affinity", new JArray(session, _vm ?? ""), serializer);
        }

        public long vm_get_memory_overhead(string session, string _vm)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<long>("VM.get_memory_overhead", new JArray(session, _vm ?? ""), serializer);
        }

        public long vm_get_memory_target(string session, string _vm)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<long>("VM.get_memory_target", new JArray(session, _vm ?? ""), serializer);
        }

        public long vm_get_memory_static_max(string session, string _vm)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<long>("VM.get_memory_static_max", new JArray(session, _vm ?? ""), serializer);
        }

        public long vm_get_memory_dynamic_max(string session, string _vm)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<long>("VM.get_memory_dynamic_max", new JArray(session, _vm ?? ""), serializer);
        }

        public long vm_get_memory_dynamic_min(string session, string _vm)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<long>("VM.get_memory_dynamic_min", new JArray(session, _vm ?? ""), serializer);
        }

        public long vm_get_memory_static_min(string session, string _vm)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<long>("VM.get_memory_static_min", new JArray(session, _vm ?? ""), serializer);
        }

        public Dictionary<string, string> vm_get_vcpus_params(string session, string _vm)
        {
            var converters = new List<JsonConverter> {new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<string, string>>("VM.get_VCPUs_params", new JArray(session, _vm ?? ""), serializer);
        }

        public long vm_get_vcpus_max(string session, string _vm)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<long>("VM.get_VCPUs_max", new JArray(session, _vm ?? ""), serializer);
        }

        public long vm_get_vcpus_at_startup(string session, string _vm)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<long>("VM.get_VCPUs_at_startup", new JArray(session, _vm ?? ""), serializer);
        }

        public on_normal_exit vm_get_actions_after_shutdown(string session, string _vm)
        {
            var converters = new List<JsonConverter> {new on_normal_exitConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<on_normal_exit>("VM.get_actions_after_shutdown", new JArray(session, _vm ?? ""), serializer);
        }

        public on_normal_exit vm_get_actions_after_reboot(string session, string _vm)
        {
            var converters = new List<JsonConverter> {new on_normal_exitConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<on_normal_exit>("VM.get_actions_after_reboot", new JArray(session, _vm ?? ""), serializer);
        }

        public on_crash_behaviour vm_get_actions_after_crash(string session, string _vm)
        {
            var converters = new List<JsonConverter> {new on_crash_behaviourConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<on_crash_behaviour>("VM.get_actions_after_crash", new JArray(session, _vm ?? ""), serializer);
        }

        public List<XenRef<Console>> vm_get_consoles(string session, string _vm)
        {
            var converters = new List<JsonConverter> {new XenRefListConverter<Console>()};
            var serializer = CreateSerializer(converters);
            return Rpc<List<XenRef<Console>>>("VM.get_consoles", new JArray(session, _vm ?? ""), serializer);
        }

        public List<XenRef<VIF>> vm_get_vifs(string session, string _vm)
        {
            var converters = new List<JsonConverter> {new XenRefListConverter<VIF>()};
            var serializer = CreateSerializer(converters);
            return Rpc<List<XenRef<VIF>>>("VM.get_VIFs", new JArray(session, _vm ?? ""), serializer);
        }

        public List<XenRef<VBD>> vm_get_vbds(string session, string _vm)
        {
            var converters = new List<JsonConverter> {new XenRefListConverter<VBD>()};
            var serializer = CreateSerializer(converters);
            return Rpc<List<XenRef<VBD>>>("VM.get_VBDs", new JArray(session, _vm ?? ""), serializer);
        }

        public List<XenRef<VUSB>> vm_get_vusbs(string session, string _vm)
        {
            var converters = new List<JsonConverter> {new XenRefListConverter<VUSB>()};
            var serializer = CreateSerializer(converters);
            return Rpc<List<XenRef<VUSB>>>("VM.get_VUSBs", new JArray(session, _vm ?? ""), serializer);
        }

        public List<XenRef<Crashdump>> vm_get_crash_dumps(string session, string _vm)
        {
            var converters = new List<JsonConverter> {new XenRefListConverter<Crashdump>()};
            var serializer = CreateSerializer(converters);
            return Rpc<List<XenRef<Crashdump>>>("VM.get_crash_dumps", new JArray(session, _vm ?? ""), serializer);
        }

        public List<XenRef<VTPM>> vm_get_vtpms(string session, string _vm)
        {
            var converters = new List<JsonConverter> {new XenRefListConverter<VTPM>()};
            var serializer = CreateSerializer(converters);
            return Rpc<List<XenRef<VTPM>>>("VM.get_VTPMs", new JArray(session, _vm ?? ""), serializer);
        }

        public string vm_get_pv_bootloader(string session, string _vm)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("VM.get_PV_bootloader", new JArray(session, _vm ?? ""), serializer);
        }

        public string vm_get_pv_kernel(string session, string _vm)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("VM.get_PV_kernel", new JArray(session, _vm ?? ""), serializer);
        }

        public string vm_get_pv_ramdisk(string session, string _vm)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("VM.get_PV_ramdisk", new JArray(session, _vm ?? ""), serializer);
        }

        public string vm_get_pv_args(string session, string _vm)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("VM.get_PV_args", new JArray(session, _vm ?? ""), serializer);
        }

        public string vm_get_pv_bootloader_args(string session, string _vm)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("VM.get_PV_bootloader_args", new JArray(session, _vm ?? ""), serializer);
        }

        public string vm_get_pv_legacy_args(string session, string _vm)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("VM.get_PV_legacy_args", new JArray(session, _vm ?? ""), serializer);
        }

        public string vm_get_hvm_boot_policy(string session, string _vm)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("VM.get_HVM_boot_policy", new JArray(session, _vm ?? ""), serializer);
        }

        public Dictionary<string, string> vm_get_hvm_boot_params(string session, string _vm)
        {
            var converters = new List<JsonConverter> {new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<string, string>>("VM.get_HVM_boot_params", new JArray(session, _vm ?? ""), serializer);
        }

        public double vm_get_hvm_shadow_multiplier(string session, string _vm)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<double>("VM.get_HVM_shadow_multiplier", new JArray(session, _vm ?? ""), serializer);
        }

        public Dictionary<string, string> vm_get_platform(string session, string _vm)
        {
            var converters = new List<JsonConverter> {new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<string, string>>("VM.get_platform", new JArray(session, _vm ?? ""), serializer);
        }

        public string vm_get_pci_bus(string session, string _vm)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("VM.get_PCI_bus", new JArray(session, _vm ?? ""), serializer);
        }

        public Dictionary<string, string> vm_get_other_config(string session, string _vm)
        {
            var converters = new List<JsonConverter> {new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<string, string>>("VM.get_other_config", new JArray(session, _vm ?? ""), serializer);
        }

        public long vm_get_domid(string session, string _vm)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<long>("VM.get_domid", new JArray(session, _vm ?? ""), serializer);
        }

        public string vm_get_domarch(string session, string _vm)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("VM.get_domarch", new JArray(session, _vm ?? ""), serializer);
        }

        public Dictionary<string, string> vm_get_last_boot_cpu_flags(string session, string _vm)
        {
            var converters = new List<JsonConverter> {new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<string, string>>("VM.get_last_boot_CPU_flags", new JArray(session, _vm ?? ""), serializer);
        }

        public bool vm_get_is_control_domain(string session, string _vm)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<bool>("VM.get_is_control_domain", new JArray(session, _vm ?? ""), serializer);
        }

        public XenRef<VM_metrics> vm_get_metrics(string session, string _vm)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<VM_metrics>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<VM_metrics>>("VM.get_metrics", new JArray(session, _vm ?? ""), serializer);
        }

        public XenRef<VM_guest_metrics> vm_get_guest_metrics(string session, string _vm)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<VM_guest_metrics>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<VM_guest_metrics>>("VM.get_guest_metrics", new JArray(session, _vm ?? ""), serializer);
        }

        public string vm_get_last_booted_record(string session, string _vm)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("VM.get_last_booted_record", new JArray(session, _vm ?? ""), serializer);
        }

        public string vm_get_recommendations(string session, string _vm)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("VM.get_recommendations", new JArray(session, _vm ?? ""), serializer);
        }

        public Dictionary<string, string> vm_get_xenstore_data(string session, string _vm)
        {
            var converters = new List<JsonConverter> {new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<string, string>>("VM.get_xenstore_data", new JArray(session, _vm ?? ""), serializer);
        }

        public bool vm_get_ha_always_run(string session, string _vm)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<bool>("VM.get_ha_always_run", new JArray(session, _vm ?? ""), serializer);
        }

        public string vm_get_ha_restart_priority(string session, string _vm)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("VM.get_ha_restart_priority", new JArray(session, _vm ?? ""), serializer);
        }

        public bool vm_get_is_a_snapshot(string session, string _vm)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<bool>("VM.get_is_a_snapshot", new JArray(session, _vm ?? ""), serializer);
        }

        public XenRef<VM> vm_get_snapshot_of(string session, string _vm)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<VM>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<VM>>("VM.get_snapshot_of", new JArray(session, _vm ?? ""), serializer);
        }

        public List<XenRef<VM>> vm_get_snapshots(string session, string _vm)
        {
            var converters = new List<JsonConverter> {new XenRefListConverter<VM>()};
            var serializer = CreateSerializer(converters);
            return Rpc<List<XenRef<VM>>>("VM.get_snapshots", new JArray(session, _vm ?? ""), serializer);
        }

        public DateTime vm_get_snapshot_time(string session, string _vm)
        {
            var converters = new List<JsonConverter> {new XenDateTimeConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<DateTime>("VM.get_snapshot_time", new JArray(session, _vm ?? ""), serializer);
        }

        public string vm_get_transportable_snapshot_id(string session, string _vm)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("VM.get_transportable_snapshot_id", new JArray(session, _vm ?? ""), serializer);
        }

        public Dictionary<string, XenRef<Blob>> vm_get_blobs(string session, string _vm)
        {
            var converters = new List<JsonConverter> {new StringXenRefMapConverter<Blob>()};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<string, XenRef<Blob>>>("VM.get_blobs", new JArray(session, _vm ?? ""), serializer);
        }

        public string[] vm_get_tags(string session, string _vm)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string[]>("VM.get_tags", new JArray(session, _vm ?? ""), serializer);
        }

        public Dictionary<vm_operations, string> vm_get_blocked_operations(string session, string _vm)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<vm_operations, string>>("VM.get_blocked_operations", new JArray(session, _vm ?? ""), serializer);
        }

        public Dictionary<string, string> vm_get_snapshot_info(string session, string _vm)
        {
            var converters = new List<JsonConverter> {new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<string, string>>("VM.get_snapshot_info", new JArray(session, _vm ?? ""), serializer);
        }

        public string vm_get_snapshot_metadata(string session, string _vm)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("VM.get_snapshot_metadata", new JArray(session, _vm ?? ""), serializer);
        }

        public XenRef<VM> vm_get_parent(string session, string _vm)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<VM>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<VM>>("VM.get_parent", new JArray(session, _vm ?? ""), serializer);
        }

        public List<XenRef<VM>> vm_get_children(string session, string _vm)
        {
            var converters = new List<JsonConverter> {new XenRefListConverter<VM>()};
            var serializer = CreateSerializer(converters);
            return Rpc<List<XenRef<VM>>>("VM.get_children", new JArray(session, _vm ?? ""), serializer);
        }

        public Dictionary<string, string> vm_get_bios_strings(string session, string _vm)
        {
            var converters = new List<JsonConverter> {new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<string, string>>("VM.get_bios_strings", new JArray(session, _vm ?? ""), serializer);
        }

        public XenRef<VMPP> vm_get_protection_policy(string session, string _vm)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<VMPP>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<VMPP>>("VM.get_protection_policy", new JArray(session, _vm ?? ""), serializer);
        }

        public bool vm_get_is_snapshot_from_vmpp(string session, string _vm)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<bool>("VM.get_is_snapshot_from_vmpp", new JArray(session, _vm ?? ""), serializer);
        }

        public XenRef<VMSS> vm_get_snapshot_schedule(string session, string _vm)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<VMSS>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<VMSS>>("VM.get_snapshot_schedule", new JArray(session, _vm ?? ""), serializer);
        }

        public bool vm_get_is_vmss_snapshot(string session, string _vm)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<bool>("VM.get_is_vmss_snapshot", new JArray(session, _vm ?? ""), serializer);
        }

        public XenRef<VM_appliance> vm_get_appliance(string session, string _vm)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<VM_appliance>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<VM_appliance>>("VM.get_appliance", new JArray(session, _vm ?? ""), serializer);
        }

        public long vm_get_start_delay(string session, string _vm)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<long>("VM.get_start_delay", new JArray(session, _vm ?? ""), serializer);
        }

        public long vm_get_shutdown_delay(string session, string _vm)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<long>("VM.get_shutdown_delay", new JArray(session, _vm ?? ""), serializer);
        }

        public long vm_get_order(string session, string _vm)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<long>("VM.get_order", new JArray(session, _vm ?? ""), serializer);
        }

        public List<XenRef<VGPU>> vm_get_vgpus(string session, string _vm)
        {
            var converters = new List<JsonConverter> {new XenRefListConverter<VGPU>()};
            var serializer = CreateSerializer(converters);
            return Rpc<List<XenRef<VGPU>>>("VM.get_VGPUs", new JArray(session, _vm ?? ""), serializer);
        }

        public List<XenRef<PCI>> vm_get_attached_pcis(string session, string _vm)
        {
            var converters = new List<JsonConverter> {new XenRefListConverter<PCI>()};
            var serializer = CreateSerializer(converters);
            return Rpc<List<XenRef<PCI>>>("VM.get_attached_PCIs", new JArray(session, _vm ?? ""), serializer);
        }

        public XenRef<SR> vm_get_suspend_sr(string session, string _vm)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<SR>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<SR>>("VM.get_suspend_SR", new JArray(session, _vm ?? ""), serializer);
        }

        public long vm_get_version(string session, string _vm)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<long>("VM.get_version", new JArray(session, _vm ?? ""), serializer);
        }

        public string vm_get_generation_id(string session, string _vm)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("VM.get_generation_id", new JArray(session, _vm ?? ""), serializer);
        }

        public long vm_get_hardware_platform_version(string session, string _vm)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<long>("VM.get_hardware_platform_version", new JArray(session, _vm ?? ""), serializer);
        }

        public bool vm_get_has_vendor_device(string session, string _vm)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<bool>("VM.get_has_vendor_device", new JArray(session, _vm ?? ""), serializer);
        }

        public bool vm_get_requires_reboot(string session, string _vm)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<bool>("VM.get_requires_reboot", new JArray(session, _vm ?? ""), serializer);
        }

        public string vm_get_reference_label(string session, string _vm)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("VM.get_reference_label", new JArray(session, _vm ?? ""), serializer);
        }

        public domain_type vm_get_domain_type(string session, string _vm)
        {
            var converters = new List<JsonConverter> {new domain_typeConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<domain_type>("VM.get_domain_type", new JArray(session, _vm ?? ""), serializer);
        }

        public void vm_set_name_label(string session, string _vm, string _label)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("VM.set_name_label", new JArray(session, _vm ?? "", _label ?? ""), serializer);
        }

        public void vm_set_name_description(string session, string _vm, string _description)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("VM.set_name_description", new JArray(session, _vm ?? "", _description ?? ""), serializer);
        }

        public void vm_set_user_version(string session, string _vm, long _user_version)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("VM.set_user_version", new JArray(session, _vm ?? "", _user_version), serializer);
        }

        public void vm_set_is_a_template(string session, string _vm, bool _is_a_template)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("VM.set_is_a_template", new JArray(session, _vm ?? "", _is_a_template), serializer);
        }

        public void vm_set_affinity(string session, string _vm, string _affinity)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Host>()};
            var serializer = CreateSerializer(converters);
            Rpc("VM.set_affinity", new JArray(session, _vm ?? "", _affinity ?? ""), serializer);
        }

        public void vm_set_vcpus_params(string session, string _vm, Dictionary<string, string> _params)
        {
            var converters = new List<JsonConverter> {new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            Rpc("VM.set_VCPUs_params", new JArray(session, _vm ?? "", _params == null ? new JObject() : JObject.FromObject(_params, serializer)), serializer);
        }

        public void vm_add_to_vcpus_params(string session, string _vm, string _key, string _value)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("VM.add_to_VCPUs_params", new JArray(session, _vm ?? "", _key ?? "", _value ?? ""), serializer);
        }

        public void vm_remove_from_vcpus_params(string session, string _vm, string _key)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("VM.remove_from_VCPUs_params", new JArray(session, _vm ?? "", _key ?? ""), serializer);
        }

        public void vm_set_actions_after_shutdown(string session, string _vm, on_normal_exit _after_shutdown)
        {
            var converters = new List<JsonConverter> {new on_normal_exitConverter()};
            var serializer = CreateSerializer(converters);
            Rpc("VM.set_actions_after_shutdown", new JArray(session, _vm ?? "", _after_shutdown.StringOf()), serializer);
        }

        public void vm_set_actions_after_reboot(string session, string _vm, on_normal_exit _after_reboot)
        {
            var converters = new List<JsonConverter> {new on_normal_exitConverter()};
            var serializer = CreateSerializer(converters);
            Rpc("VM.set_actions_after_reboot", new JArray(session, _vm ?? "", _after_reboot.StringOf()), serializer);
        }

        public void vm_set_pv_bootloader(string session, string _vm, string _bootloader)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("VM.set_PV_bootloader", new JArray(session, _vm ?? "", _bootloader ?? ""), serializer);
        }

        public void vm_set_pv_kernel(string session, string _vm, string _kernel)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("VM.set_PV_kernel", new JArray(session, _vm ?? "", _kernel ?? ""), serializer);
        }

        public void vm_set_pv_ramdisk(string session, string _vm, string _ramdisk)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("VM.set_PV_ramdisk", new JArray(session, _vm ?? "", _ramdisk ?? ""), serializer);
        }

        public void vm_set_pv_args(string session, string _vm, string _args)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("VM.set_PV_args", new JArray(session, _vm ?? "", _args ?? ""), serializer);
        }

        public void vm_set_pv_bootloader_args(string session, string _vm, string _bootloader_args)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("VM.set_PV_bootloader_args", new JArray(session, _vm ?? "", _bootloader_args ?? ""), serializer);
        }

        public void vm_set_pv_legacy_args(string session, string _vm, string _legacy_args)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("VM.set_PV_legacy_args", new JArray(session, _vm ?? "", _legacy_args ?? ""), serializer);
        }

        public void vm_set_hvm_boot_params(string session, string _vm, Dictionary<string, string> _boot_params)
        {
            var converters = new List<JsonConverter> {new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            Rpc("VM.set_HVM_boot_params", new JArray(session, _vm ?? "", _boot_params == null ? new JObject() : JObject.FromObject(_boot_params, serializer)), serializer);
        }

        public void vm_add_to_hvm_boot_params(string session, string _vm, string _key, string _value)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("VM.add_to_HVM_boot_params", new JArray(session, _vm ?? "", _key ?? "", _value ?? ""), serializer);
        }

        public void vm_remove_from_hvm_boot_params(string session, string _vm, string _key)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("VM.remove_from_HVM_boot_params", new JArray(session, _vm ?? "", _key ?? ""), serializer);
        }

        public void vm_set_platform(string session, string _vm, Dictionary<string, string> _platform)
        {
            var converters = new List<JsonConverter> {new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            Rpc("VM.set_platform", new JArray(session, _vm ?? "", _platform == null ? new JObject() : JObject.FromObject(_platform, serializer)), serializer);
        }

        public void vm_add_to_platform(string session, string _vm, string _key, string _value)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("VM.add_to_platform", new JArray(session, _vm ?? "", _key ?? "", _value ?? ""), serializer);
        }

        public void vm_remove_from_platform(string session, string _vm, string _key)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("VM.remove_from_platform", new JArray(session, _vm ?? "", _key ?? ""), serializer);
        }

        public void vm_set_pci_bus(string session, string _vm, string _pci_bus)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("VM.set_PCI_bus", new JArray(session, _vm ?? "", _pci_bus ?? ""), serializer);
        }

        public void vm_set_other_config(string session, string _vm, Dictionary<string, string> _other_config)
        {
            var converters = new List<JsonConverter> {new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            Rpc("VM.set_other_config", new JArray(session, _vm ?? "", _other_config == null ? new JObject() : JObject.FromObject(_other_config, serializer)), serializer);
        }

        public void vm_add_to_other_config(string session, string _vm, string _key, string _value)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("VM.add_to_other_config", new JArray(session, _vm ?? "", _key ?? "", _value ?? ""), serializer);
        }

        public void vm_remove_from_other_config(string session, string _vm, string _key)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("VM.remove_from_other_config", new JArray(session, _vm ?? "", _key ?? ""), serializer);
        }

        public void vm_set_recommendations(string session, string _vm, string _recommendations)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("VM.set_recommendations", new JArray(session, _vm ?? "", _recommendations ?? ""), serializer);
        }

        public void vm_set_xenstore_data(string session, string _vm, Dictionary<string, string> _xenstore_data)
        {
            var converters = new List<JsonConverter> {new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            Rpc("VM.set_xenstore_data", new JArray(session, _vm ?? "", _xenstore_data == null ? new JObject() : JObject.FromObject(_xenstore_data, serializer)), serializer);
        }

        public void vm_add_to_xenstore_data(string session, string _vm, string _key, string _value)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("VM.add_to_xenstore_data", new JArray(session, _vm ?? "", _key ?? "", _value ?? ""), serializer);
        }

        public void vm_remove_from_xenstore_data(string session, string _vm, string _key)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("VM.remove_from_xenstore_data", new JArray(session, _vm ?? "", _key ?? ""), serializer);
        }

        public void vm_set_tags(string session, string _vm, string[] _tags)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("VM.set_tags", new JArray(session, _vm ?? "", _tags == null ? new JArray() : JArray.FromObject(_tags)), serializer);
        }

        public void vm_add_tags(string session, string _vm, string _value)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("VM.add_tags", new JArray(session, _vm ?? "", _value ?? ""), serializer);
        }

        public void vm_remove_tags(string session, string _vm, string _value)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("VM.remove_tags", new JArray(session, _vm ?? "", _value ?? ""), serializer);
        }

        public void vm_set_blocked_operations(string session, string _vm, Dictionary<vm_operations, string> _blocked_operations)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("VM.set_blocked_operations", new JArray(session, _vm ?? "", _blocked_operations == null ? new JObject() : JObject.FromObject(_blocked_operations, serializer)), serializer);
        }

        public void vm_add_to_blocked_operations(string session, string _vm, vm_operations _key, string _value)
        {
            var converters = new List<JsonConverter> {new vm_operationsConverter()};
            var serializer = CreateSerializer(converters);
            Rpc("VM.add_to_blocked_operations", new JArray(session, _vm ?? "", _key.StringOf(), _value ?? ""), serializer);
        }

        public void vm_remove_from_blocked_operations(string session, string _vm, vm_operations _key)
        {
            var converters = new List<JsonConverter> {new vm_operationsConverter()};
            var serializer = CreateSerializer(converters);
            Rpc("VM.remove_from_blocked_operations", new JArray(session, _vm ?? "", _key.StringOf()), serializer);
        }

        public void vm_set_suspend_sr(string session, string _vm, string _suspend_sr)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<SR>()};
            var serializer = CreateSerializer(converters);
            Rpc("VM.set_suspend_SR", new JArray(session, _vm ?? "", _suspend_sr ?? ""), serializer);
        }

        public void vm_set_hardware_platform_version(string session, string _vm, long _hardware_platform_version)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("VM.set_hardware_platform_version", new JArray(session, _vm ?? "", _hardware_platform_version), serializer);
        }

        public XenRef<VM> vm_snapshot(string session, string _vm, string _new_name)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<VM>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<VM>>("VM.snapshot", new JArray(session, _vm ?? "", _new_name ?? ""), serializer);
        }

        public XenRef<Task> async_vm_snapshot(string session, string _vm, string _new_name)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.VM.snapshot", new JArray(session, _vm ?? "", _new_name ?? ""), serializer);
        }

        public XenRef<VM> vm_snapshot_with_quiesce(string session, string _vm, string _new_name)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<VM>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<VM>>("VM.snapshot_with_quiesce", new JArray(session, _vm ?? "", _new_name ?? ""), serializer);
        }

        public XenRef<Task> async_vm_snapshot_with_quiesce(string session, string _vm, string _new_name)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.VM.snapshot_with_quiesce", new JArray(session, _vm ?? "", _new_name ?? ""), serializer);
        }

        public XenRef<VM> vm_clone(string session, string _vm, string _new_name)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<VM>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<VM>>("VM.clone", new JArray(session, _vm ?? "", _new_name ?? ""), serializer);
        }

        public XenRef<Task> async_vm_clone(string session, string _vm, string _new_name)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.VM.clone", new JArray(session, _vm ?? "", _new_name ?? ""), serializer);
        }

        public XenRef<VM> vm_copy(string session, string _vm, string _new_name, string _sr)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<VM>(), new XenRefConverter<SR>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<VM>>("VM.copy", new JArray(session, _vm ?? "", _new_name ?? "", _sr ?? ""), serializer);
        }

        public XenRef<Task> async_vm_copy(string session, string _vm, string _new_name, string _sr)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>(), new XenRefConverter<SR>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.VM.copy", new JArray(session, _vm ?? "", _new_name ?? "", _sr ?? ""), serializer);
        }

        public void vm_revert(string session, string _vm)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("VM.revert", new JArray(session, _vm ?? ""), serializer);
        }

        public XenRef<Task> async_vm_revert(string session, string _vm)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.VM.revert", new JArray(session, _vm ?? ""), serializer);
        }

        public XenRef<VM> vm_checkpoint(string session, string _vm, string _new_name)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<VM>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<VM>>("VM.checkpoint", new JArray(session, _vm ?? "", _new_name ?? ""), serializer);
        }

        public XenRef<Task> async_vm_checkpoint(string session, string _vm, string _new_name)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.VM.checkpoint", new JArray(session, _vm ?? "", _new_name ?? ""), serializer);
        }

        public void vm_provision(string session, string _vm)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("VM.provision", new JArray(session, _vm ?? ""), serializer);
        }

        public XenRef<Task> async_vm_provision(string session, string _vm)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.VM.provision", new JArray(session, _vm ?? ""), serializer);
        }

        public void vm_start(string session, string _vm, bool _start_paused, bool _force)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("VM.start", new JArray(session, _vm ?? "", _start_paused, _force), serializer);
        }

        public XenRef<Task> async_vm_start(string session, string _vm, bool _start_paused, bool _force)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.VM.start", new JArray(session, _vm ?? "", _start_paused, _force), serializer);
        }

        public void vm_start_on(string session, string _vm, string _host, bool _start_paused, bool _force)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Host>()};
            var serializer = CreateSerializer(converters);
            Rpc("VM.start_on", new JArray(session, _vm ?? "", _host ?? "", _start_paused, _force), serializer);
        }

        public XenRef<Task> async_vm_start_on(string session, string _vm, string _host, bool _start_paused, bool _force)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>(), new XenRefConverter<Host>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.VM.start_on", new JArray(session, _vm ?? "", _host ?? "", _start_paused, _force), serializer);
        }

        public void vm_pause(string session, string _vm)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("VM.pause", new JArray(session, _vm ?? ""), serializer);
        }

        public XenRef<Task> async_vm_pause(string session, string _vm)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.VM.pause", new JArray(session, _vm ?? ""), serializer);
        }

        public void vm_unpause(string session, string _vm)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("VM.unpause", new JArray(session, _vm ?? ""), serializer);
        }

        public XenRef<Task> async_vm_unpause(string session, string _vm)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.VM.unpause", new JArray(session, _vm ?? ""), serializer);
        }

        public void vm_clean_shutdown(string session, string _vm)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("VM.clean_shutdown", new JArray(session, _vm ?? ""), serializer);
        }

        public XenRef<Task> async_vm_clean_shutdown(string session, string _vm)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.VM.clean_shutdown", new JArray(session, _vm ?? ""), serializer);
        }

        public void vm_shutdown(string session, string _vm)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("VM.shutdown", new JArray(session, _vm ?? ""), serializer);
        }

        public XenRef<Task> async_vm_shutdown(string session, string _vm)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.VM.shutdown", new JArray(session, _vm ?? ""), serializer);
        }

        public void vm_clean_reboot(string session, string _vm)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("VM.clean_reboot", new JArray(session, _vm ?? ""), serializer);
        }

        public XenRef<Task> async_vm_clean_reboot(string session, string _vm)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.VM.clean_reboot", new JArray(session, _vm ?? ""), serializer);
        }

        public void vm_hard_shutdown(string session, string _vm)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("VM.hard_shutdown", new JArray(session, _vm ?? ""), serializer);
        }

        public XenRef<Task> async_vm_hard_shutdown(string session, string _vm)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.VM.hard_shutdown", new JArray(session, _vm ?? ""), serializer);
        }

        public void vm_power_state_reset(string session, string _vm)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("VM.power_state_reset", new JArray(session, _vm ?? ""), serializer);
        }

        public XenRef<Task> async_vm_power_state_reset(string session, string _vm)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.VM.power_state_reset", new JArray(session, _vm ?? ""), serializer);
        }

        public void vm_hard_reboot(string session, string _vm)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("VM.hard_reboot", new JArray(session, _vm ?? ""), serializer);
        }

        public XenRef<Task> async_vm_hard_reboot(string session, string _vm)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.VM.hard_reboot", new JArray(session, _vm ?? ""), serializer);
        }

        public void vm_suspend(string session, string _vm)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("VM.suspend", new JArray(session, _vm ?? ""), serializer);
        }

        public XenRef<Task> async_vm_suspend(string session, string _vm)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.VM.suspend", new JArray(session, _vm ?? ""), serializer);
        }

        public void vm_resume(string session, string _vm, bool _start_paused, bool _force)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("VM.resume", new JArray(session, _vm ?? "", _start_paused, _force), serializer);
        }

        public XenRef<Task> async_vm_resume(string session, string _vm, bool _start_paused, bool _force)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.VM.resume", new JArray(session, _vm ?? "", _start_paused, _force), serializer);
        }

        public void vm_resume_on(string session, string _vm, string _host, bool _start_paused, bool _force)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Host>()};
            var serializer = CreateSerializer(converters);
            Rpc("VM.resume_on", new JArray(session, _vm ?? "", _host ?? "", _start_paused, _force), serializer);
        }

        public XenRef<Task> async_vm_resume_on(string session, string _vm, string _host, bool _start_paused, bool _force)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>(), new XenRefConverter<Host>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.VM.resume_on", new JArray(session, _vm ?? "", _host ?? "", _start_paused, _force), serializer);
        }

        public void vm_pool_migrate(string session, string _vm, string _host, Dictionary<string, string> _options)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Host>(), new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            Rpc("VM.pool_migrate", new JArray(session, _vm ?? "", _host ?? "", _options == null ? new JObject() : JObject.FromObject(_options, serializer)), serializer);
        }

        public XenRef<Task> async_vm_pool_migrate(string session, string _vm, string _host, Dictionary<string, string> _options)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>(), new XenRefConverter<Host>(), new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.VM.pool_migrate", new JArray(session, _vm ?? "", _host ?? "", _options == null ? new JObject() : JObject.FromObject(_options, serializer)), serializer);
        }

        public void vm_set_vcpus_number_live(string session, string _vm, long _nvcpu)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("VM.set_VCPUs_number_live", new JArray(session, _vm ?? "", _nvcpu), serializer);
        }

        public XenRef<Task> async_vm_set_vcpus_number_live(string session, string _vm, long _nvcpu)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.VM.set_VCPUs_number_live", new JArray(session, _vm ?? "", _nvcpu), serializer);
        }

        public void vm_add_to_vcpus_params_live(string session, string _vm, string _key, string _value)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("VM.add_to_VCPUs_params_live", new JArray(session, _vm ?? "", _key ?? "", _value ?? ""), serializer);
        }

        public XenRef<Task> async_vm_add_to_vcpus_params_live(string session, string _vm, string _key, string _value)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.VM.add_to_VCPUs_params_live", new JArray(session, _vm ?? "", _key ?? "", _value ?? ""), serializer);
        }

        public void vm_set_ha_restart_priority(string session, string _vm, string _value)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("VM.set_ha_restart_priority", new JArray(session, _vm ?? "", _value ?? ""), serializer);
        }

        public void vm_set_ha_always_run(string session, string _vm, bool _value)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("VM.set_ha_always_run", new JArray(session, _vm ?? "", _value), serializer);
        }

        public long vm_compute_memory_overhead(string session, string _vm)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<long>("VM.compute_memory_overhead", new JArray(session, _vm ?? ""), serializer);
        }

        public XenRef<Task> async_vm_compute_memory_overhead(string session, string _vm)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.VM.compute_memory_overhead", new JArray(session, _vm ?? ""), serializer);
        }

        public void vm_set_memory_dynamic_max(string session, string _vm, long _value)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("VM.set_memory_dynamic_max", new JArray(session, _vm ?? "", _value), serializer);
        }

        public void vm_set_memory_dynamic_min(string session, string _vm, long _value)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("VM.set_memory_dynamic_min", new JArray(session, _vm ?? "", _value), serializer);
        }

        public void vm_set_memory_dynamic_range(string session, string _vm, long _min, long _max)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("VM.set_memory_dynamic_range", new JArray(session, _vm ?? "", _min, _max), serializer);
        }

        public XenRef<Task> async_vm_set_memory_dynamic_range(string session, string _vm, long _min, long _max)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.VM.set_memory_dynamic_range", new JArray(session, _vm ?? "", _min, _max), serializer);
        }

        public void vm_set_memory_static_max(string session, string _vm, long _value)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("VM.set_memory_static_max", new JArray(session, _vm ?? "", _value), serializer);
        }

        public void vm_set_memory_static_min(string session, string _vm, long _value)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("VM.set_memory_static_min", new JArray(session, _vm ?? "", _value), serializer);
        }

        public void vm_set_memory_static_range(string session, string _vm, long _min, long _max)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("VM.set_memory_static_range", new JArray(session, _vm ?? "", _min, _max), serializer);
        }

        public XenRef<Task> async_vm_set_memory_static_range(string session, string _vm, long _min, long _max)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.VM.set_memory_static_range", new JArray(session, _vm ?? "", _min, _max), serializer);
        }

        public void vm_set_memory_limits(string session, string _vm, long _static_min, long _static_max, long _dynamic_min, long _dynamic_max)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("VM.set_memory_limits", new JArray(session, _vm ?? "", _static_min, _static_max, _dynamic_min, _dynamic_max), serializer);
        }

        public XenRef<Task> async_vm_set_memory_limits(string session, string _vm, long _static_min, long _static_max, long _dynamic_min, long _dynamic_max)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.VM.set_memory_limits", new JArray(session, _vm ?? "", _static_min, _static_max, _dynamic_min, _dynamic_max), serializer);
        }

        public void vm_set_memory(string session, string _vm, long _value)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("VM.set_memory", new JArray(session, _vm ?? "", _value), serializer);
        }

        public XenRef<Task> async_vm_set_memory(string session, string _vm, long _value)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.VM.set_memory", new JArray(session, _vm ?? "", _value), serializer);
        }

        public void vm_set_memory_target_live(string session, string _vm, long _target)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("VM.set_memory_target_live", new JArray(session, _vm ?? "", _target), serializer);
        }

        public XenRef<Task> async_vm_set_memory_target_live(string session, string _vm, long _target)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.VM.set_memory_target_live", new JArray(session, _vm ?? "", _target), serializer);
        }

        public void vm_wait_memory_target_live(string session, string _vm)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("VM.wait_memory_target_live", new JArray(session, _vm ?? ""), serializer);
        }

        public XenRef<Task> async_vm_wait_memory_target_live(string session, string _vm)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.VM.wait_memory_target_live", new JArray(session, _vm ?? ""), serializer);
        }

        public bool vm_get_cooperative(string session, string _vm)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<bool>("VM.get_cooperative", new JArray(session, _vm ?? ""), serializer);
        }

        public XenRef<Task> async_vm_get_cooperative(string session, string _vm)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.VM.get_cooperative", new JArray(session, _vm ?? ""), serializer);
        }

        public void vm_set_hvm_shadow_multiplier(string session, string _vm, double _value)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("VM.set_HVM_shadow_multiplier", new JArray(session, _vm ?? "", _value), serializer);
        }

        public void vm_set_shadow_multiplier_live(string session, string _vm, double _multiplier)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("VM.set_shadow_multiplier_live", new JArray(session, _vm ?? "", _multiplier), serializer);
        }

        public XenRef<Task> async_vm_set_shadow_multiplier_live(string session, string _vm, double _multiplier)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.VM.set_shadow_multiplier_live", new JArray(session, _vm ?? "", _multiplier), serializer);
        }

        public void vm_set_vcpus_max(string session, string _vm, long _value)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("VM.set_VCPUs_max", new JArray(session, _vm ?? "", _value), serializer);
        }

        public void vm_set_vcpus_at_startup(string session, string _vm, long _value)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("VM.set_VCPUs_at_startup", new JArray(session, _vm ?? "", _value), serializer);
        }

        public void vm_send_sysrq(string session, string _vm, string _key)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("VM.send_sysrq", new JArray(session, _vm ?? "", _key ?? ""), serializer);
        }

        public XenRef<Task> async_vm_send_sysrq(string session, string _vm, string _key)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.VM.send_sysrq", new JArray(session, _vm ?? "", _key ?? ""), serializer);
        }

        public void vm_send_trigger(string session, string _vm, string _trigger)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("VM.send_trigger", new JArray(session, _vm ?? "", _trigger ?? ""), serializer);
        }

        public XenRef<Task> async_vm_send_trigger(string session, string _vm, string _trigger)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.VM.send_trigger", new JArray(session, _vm ?? "", _trigger ?? ""), serializer);
        }

        public long vm_maximise_memory(string session, string _vm, long _total, bool _approximate)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<long>("VM.maximise_memory", new JArray(session, _vm ?? "", _total, _approximate), serializer);
        }

        public XenRef<Task> async_vm_maximise_memory(string session, string _vm, long _total, bool _approximate)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.VM.maximise_memory", new JArray(session, _vm ?? "", _total, _approximate), serializer);
        }

        public XenRef<VM> vm_migrate_send(string session, string _vm, Dictionary<string, string> _dest, bool _live, Dictionary<XenRef<VDI>, XenRef<SR>> _vdi_map, Dictionary<XenRef<VIF>, XenRef<Network>> _vif_map, Dictionary<string, string> _options)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<VM>(), new StringStringMapConverter(), new XenRefXenRefMapConverter<VDI, SR>(), new XenRefXenRefMapConverter<VIF, Network>(), new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<VM>>("VM.migrate_send", new JArray(session, _vm ?? "", _dest == null ? new JObject() : JObject.FromObject(_dest, serializer), _live, _vdi_map == null ? new JObject() : JObject.FromObject(_vdi_map, serializer), _vif_map == null ? new JObject() : JObject.FromObject(_vif_map, serializer), _options == null ? new JObject() : JObject.FromObject(_options, serializer)), serializer);
        }

        public XenRef<Task> async_vm_migrate_send(string session, string _vm, Dictionary<string, string> _dest, bool _live, Dictionary<XenRef<VDI>, XenRef<SR>> _vdi_map, Dictionary<XenRef<VIF>, XenRef<Network>> _vif_map, Dictionary<string, string> _options)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>(), new StringStringMapConverter(), new XenRefXenRefMapConverter<VDI, SR>(), new XenRefXenRefMapConverter<VIF, Network>(), new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.VM.migrate_send", new JArray(session, _vm ?? "", _dest == null ? new JObject() : JObject.FromObject(_dest, serializer), _live, _vdi_map == null ? new JObject() : JObject.FromObject(_vdi_map, serializer), _vif_map == null ? new JObject() : JObject.FromObject(_vif_map, serializer), _options == null ? new JObject() : JObject.FromObject(_options, serializer)), serializer);
        }

        public XenRef<VM> vm_migrate_send(string session, string _vm, Dictionary<string, string> _dest, bool _live, Dictionary<XenRef<VDI>, XenRef<SR>> _vdi_map, Dictionary<XenRef<VIF>, XenRef<Network>> _vif_map, Dictionary<string, string> _options, Dictionary<XenRef<VGPU>, XenRef<GPU_group>> _vgpu_map)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<VM>(), new StringStringMapConverter(), new XenRefXenRefMapConverter<VDI, SR>(), new XenRefXenRefMapConverter<VIF, Network>(), new StringStringMapConverter(), new XenRefXenRefMapConverter<VGPU, GPU_group>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<VM>>("VM.migrate_send", new JArray(session, _vm ?? "", _dest == null ? new JObject() : JObject.FromObject(_dest, serializer), _live, _vdi_map == null ? new JObject() : JObject.FromObject(_vdi_map, serializer), _vif_map == null ? new JObject() : JObject.FromObject(_vif_map, serializer), _options == null ? new JObject() : JObject.FromObject(_options, serializer), _vgpu_map == null ? new JObject() : JObject.FromObject(_vgpu_map, serializer)), serializer);
        }

        public XenRef<Task> async_vm_migrate_send(string session, string _vm, Dictionary<string, string> _dest, bool _live, Dictionary<XenRef<VDI>, XenRef<SR>> _vdi_map, Dictionary<XenRef<VIF>, XenRef<Network>> _vif_map, Dictionary<string, string> _options, Dictionary<XenRef<VGPU>, XenRef<GPU_group>> _vgpu_map)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>(), new StringStringMapConverter(), new XenRefXenRefMapConverter<VDI, SR>(), new XenRefXenRefMapConverter<VIF, Network>(), new StringStringMapConverter(), new XenRefXenRefMapConverter<VGPU, GPU_group>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.VM.migrate_send", new JArray(session, _vm ?? "", _dest == null ? new JObject() : JObject.FromObject(_dest, serializer), _live, _vdi_map == null ? new JObject() : JObject.FromObject(_vdi_map, serializer), _vif_map == null ? new JObject() : JObject.FromObject(_vif_map, serializer), _options == null ? new JObject() : JObject.FromObject(_options, serializer), _vgpu_map == null ? new JObject() : JObject.FromObject(_vgpu_map, serializer)), serializer);
        }

        public void vm_assert_can_migrate(string session, string _vm, Dictionary<string, string> _dest, bool _live, Dictionary<XenRef<VDI>, XenRef<SR>> _vdi_map, Dictionary<XenRef<VIF>, XenRef<Network>> _vif_map, Dictionary<string, string> _options)
        {
            var converters = new List<JsonConverter> {new StringStringMapConverter(), new XenRefXenRefMapConverter<VDI, SR>(), new XenRefXenRefMapConverter<VIF, Network>(), new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            Rpc("VM.assert_can_migrate", new JArray(session, _vm ?? "", _dest == null ? new JObject() : JObject.FromObject(_dest, serializer), _live, _vdi_map == null ? new JObject() : JObject.FromObject(_vdi_map, serializer), _vif_map == null ? new JObject() : JObject.FromObject(_vif_map, serializer), _options == null ? new JObject() : JObject.FromObject(_options, serializer)), serializer);
        }

        public XenRef<Task> async_vm_assert_can_migrate(string session, string _vm, Dictionary<string, string> _dest, bool _live, Dictionary<XenRef<VDI>, XenRef<SR>> _vdi_map, Dictionary<XenRef<VIF>, XenRef<Network>> _vif_map, Dictionary<string, string> _options)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>(), new StringStringMapConverter(), new XenRefXenRefMapConverter<VDI, SR>(), new XenRefXenRefMapConverter<VIF, Network>(), new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.VM.assert_can_migrate", new JArray(session, _vm ?? "", _dest == null ? new JObject() : JObject.FromObject(_dest, serializer), _live, _vdi_map == null ? new JObject() : JObject.FromObject(_vdi_map, serializer), _vif_map == null ? new JObject() : JObject.FromObject(_vif_map, serializer), _options == null ? new JObject() : JObject.FromObject(_options, serializer)), serializer);
        }

        public void vm_assert_can_migrate(string session, string _vm, Dictionary<string, string> _dest, bool _live, Dictionary<XenRef<VDI>, XenRef<SR>> _vdi_map, Dictionary<XenRef<VIF>, XenRef<Network>> _vif_map, Dictionary<string, string> _options, Dictionary<XenRef<VGPU>, XenRef<GPU_group>> _vgpu_map)
        {
            var converters = new List<JsonConverter> {new StringStringMapConverter(), new XenRefXenRefMapConverter<VDI, SR>(), new XenRefXenRefMapConverter<VIF, Network>(), new StringStringMapConverter(), new XenRefXenRefMapConverter<VGPU, GPU_group>()};
            var serializer = CreateSerializer(converters);
            Rpc("VM.assert_can_migrate", new JArray(session, _vm ?? "", _dest == null ? new JObject() : JObject.FromObject(_dest, serializer), _live, _vdi_map == null ? new JObject() : JObject.FromObject(_vdi_map, serializer), _vif_map == null ? new JObject() : JObject.FromObject(_vif_map, serializer), _options == null ? new JObject() : JObject.FromObject(_options, serializer), _vgpu_map == null ? new JObject() : JObject.FromObject(_vgpu_map, serializer)), serializer);
        }

        public XenRef<Task> async_vm_assert_can_migrate(string session, string _vm, Dictionary<string, string> _dest, bool _live, Dictionary<XenRef<VDI>, XenRef<SR>> _vdi_map, Dictionary<XenRef<VIF>, XenRef<Network>> _vif_map, Dictionary<string, string> _options, Dictionary<XenRef<VGPU>, XenRef<GPU_group>> _vgpu_map)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>(), new StringStringMapConverter(), new XenRefXenRefMapConverter<VDI, SR>(), new XenRefXenRefMapConverter<VIF, Network>(), new StringStringMapConverter(), new XenRefXenRefMapConverter<VGPU, GPU_group>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.VM.assert_can_migrate", new JArray(session, _vm ?? "", _dest == null ? new JObject() : JObject.FromObject(_dest, serializer), _live, _vdi_map == null ? new JObject() : JObject.FromObject(_vdi_map, serializer), _vif_map == null ? new JObject() : JObject.FromObject(_vif_map, serializer), _options == null ? new JObject() : JObject.FromObject(_options, serializer), _vgpu_map == null ? new JObject() : JObject.FromObject(_vgpu_map, serializer)), serializer);
        }

        public VM vm_get_boot_record(string session, string _vm)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<VM>("VM.get_boot_record", new JArray(session, _vm ?? ""), serializer);
        }

        public List<Data_source> vm_get_data_sources(string session, string _vm)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<List<Data_source>>("VM.get_data_sources", new JArray(session, _vm ?? ""), serializer);
        }

        public void vm_record_data_source(string session, string _vm, string _data_source)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("VM.record_data_source", new JArray(session, _vm ?? "", _data_source ?? ""), serializer);
        }

        public double vm_query_data_source(string session, string _vm, string _data_source)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<double>("VM.query_data_source", new JArray(session, _vm ?? "", _data_source ?? ""), serializer);
        }

        public void vm_forget_data_source_archives(string session, string _vm, string _data_source)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("VM.forget_data_source_archives", new JArray(session, _vm ?? "", _data_source ?? ""), serializer);
        }

        public void vm_assert_operation_valid(string session, string _vm, vm_operations _op)
        {
            var converters = new List<JsonConverter> {new vm_operationsConverter()};
            var serializer = CreateSerializer(converters);
            Rpc("VM.assert_operation_valid", new JArray(session, _vm ?? "", _op.StringOf()), serializer);
        }

        public XenRef<Task> async_vm_assert_operation_valid(string session, string _vm, vm_operations _op)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>(), new vm_operationsConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.VM.assert_operation_valid", new JArray(session, _vm ?? "", _op.StringOf()), serializer);
        }

        public void vm_update_allowed_operations(string session, string _vm)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("VM.update_allowed_operations", new JArray(session, _vm ?? ""), serializer);
        }

        public XenRef<Task> async_vm_update_allowed_operations(string session, string _vm)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.VM.update_allowed_operations", new JArray(session, _vm ?? ""), serializer);
        }

        public string[] vm_get_allowed_vbd_devices(string session, string _vm)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string[]>("VM.get_allowed_VBD_devices", new JArray(session, _vm ?? ""), serializer);
        }

        public string[] vm_get_allowed_vif_devices(string session, string _vm)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string[]>("VM.get_allowed_VIF_devices", new JArray(session, _vm ?? ""), serializer);
        }

        public List<XenRef<Host>> vm_get_possible_hosts(string session, string _vm)
        {
            var converters = new List<JsonConverter> {new XenRefListConverter<Host>()};
            var serializer = CreateSerializer(converters);
            return Rpc<List<XenRef<Host>>>("VM.get_possible_hosts", new JArray(session, _vm ?? ""), serializer);
        }

        public XenRef<Task> async_vm_get_possible_hosts(string session, string _vm)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.VM.get_possible_hosts", new JArray(session, _vm ?? ""), serializer);
        }

        public void vm_assert_can_boot_here(string session, string _vm, string _host)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Host>()};
            var serializer = CreateSerializer(converters);
            Rpc("VM.assert_can_boot_here", new JArray(session, _vm ?? "", _host ?? ""), serializer);
        }

        public XenRef<Task> async_vm_assert_can_boot_here(string session, string _vm, string _host)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>(), new XenRefConverter<Host>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.VM.assert_can_boot_here", new JArray(session, _vm ?? "", _host ?? ""), serializer);
        }

        public XenRef<Blob> vm_create_new_blob(string session, string _vm, string _name, string _mime_type)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Blob>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Blob>>("VM.create_new_blob", new JArray(session, _vm ?? "", _name ?? "", _mime_type ?? ""), serializer);
        }

        public XenRef<Task> async_vm_create_new_blob(string session, string _vm, string _name, string _mime_type)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.VM.create_new_blob", new JArray(session, _vm ?? "", _name ?? "", _mime_type ?? ""), serializer);
        }

        public XenRef<Blob> vm_create_new_blob(string session, string _vm, string _name, string _mime_type, bool _public)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Blob>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Blob>>("VM.create_new_blob", new JArray(session, _vm ?? "", _name ?? "", _mime_type ?? "", _public), serializer);
        }

        public XenRef<Task> async_vm_create_new_blob(string session, string _vm, string _name, string _mime_type, bool _public)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.VM.create_new_blob", new JArray(session, _vm ?? "", _name ?? "", _mime_type ?? "", _public), serializer);
        }

        public void vm_assert_agile(string session, string _vm)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("VM.assert_agile", new JArray(session, _vm ?? ""), serializer);
        }

        public XenRef<Task> async_vm_assert_agile(string session, string _vm)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.VM.assert_agile", new JArray(session, _vm ?? ""), serializer);
        }

        public Dictionary<XenRef<Host>, string[]> vm_retrieve_wlb_recommendations(string session, string _vm)
        {
            var converters = new List<JsonConverter> {new XenRefStringSetMapConverter<Host>()};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<XenRef<Host>, string[]>>("VM.retrieve_wlb_recommendations", new JArray(session, _vm ?? ""), serializer);
        }

        public XenRef<Task> async_vm_retrieve_wlb_recommendations(string session, string _vm)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.VM.retrieve_wlb_recommendations", new JArray(session, _vm ?? ""), serializer);
        }

        public void vm_set_bios_strings(string session, string _vm, Dictionary<string, string> _value)
        {
            var converters = new List<JsonConverter> {new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            Rpc("VM.set_bios_strings", new JArray(session, _vm ?? "", _value == null ? new JObject() : JObject.FromObject(_value, serializer)), serializer);
        }

        public XenRef<Task> async_vm_set_bios_strings(string session, string _vm, Dictionary<string, string> _value)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>(), new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.VM.set_bios_strings", new JArray(session, _vm ?? "", _value == null ? new JObject() : JObject.FromObject(_value, serializer)), serializer);
        }

        public void vm_copy_bios_strings(string session, string _vm, string _host)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Host>()};
            var serializer = CreateSerializer(converters);
            Rpc("VM.copy_bios_strings", new JArray(session, _vm ?? "", _host ?? ""), serializer);
        }

        public XenRef<Task> async_vm_copy_bios_strings(string session, string _vm, string _host)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>(), new XenRefConverter<Host>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.VM.copy_bios_strings", new JArray(session, _vm ?? "", _host ?? ""), serializer);
        }

        public void vm_set_protection_policy(string session, string _vm, string _value)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<VMPP>()};
            var serializer = CreateSerializer(converters);
            Rpc("VM.set_protection_policy", new JArray(session, _vm ?? "", _value ?? ""), serializer);
        }

        public void vm_set_snapshot_schedule(string session, string _vm, string _value)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<VMSS>()};
            var serializer = CreateSerializer(converters);
            Rpc("VM.set_snapshot_schedule", new JArray(session, _vm ?? "", _value ?? ""), serializer);
        }

        public void vm_set_start_delay(string session, string _vm, long _value)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("VM.set_start_delay", new JArray(session, _vm ?? "", _value), serializer);
        }

        public XenRef<Task> async_vm_set_start_delay(string session, string _vm, long _value)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.VM.set_start_delay", new JArray(session, _vm ?? "", _value), serializer);
        }

        public void vm_set_shutdown_delay(string session, string _vm, long _value)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("VM.set_shutdown_delay", new JArray(session, _vm ?? "", _value), serializer);
        }

        public XenRef<Task> async_vm_set_shutdown_delay(string session, string _vm, long _value)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.VM.set_shutdown_delay", new JArray(session, _vm ?? "", _value), serializer);
        }

        public void vm_set_order(string session, string _vm, long _value)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("VM.set_order", new JArray(session, _vm ?? "", _value), serializer);
        }

        public XenRef<Task> async_vm_set_order(string session, string _vm, long _value)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.VM.set_order", new JArray(session, _vm ?? "", _value), serializer);
        }

        public void vm_set_suspend_vdi(string session, string _vm, string _value)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<VDI>()};
            var serializer = CreateSerializer(converters);
            Rpc("VM.set_suspend_VDI", new JArray(session, _vm ?? "", _value ?? ""), serializer);
        }

        public XenRef<Task> async_vm_set_suspend_vdi(string session, string _vm, string _value)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>(), new XenRefConverter<VDI>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.VM.set_suspend_VDI", new JArray(session, _vm ?? "", _value ?? ""), serializer);
        }

        public void vm_assert_can_be_recovered(string session, string _vm, string _session_to)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Session>()};
            var serializer = CreateSerializer(converters);
            Rpc("VM.assert_can_be_recovered", new JArray(session, _vm ?? "", _session_to ?? ""), serializer);
        }

        public XenRef<Task> async_vm_assert_can_be_recovered(string session, string _vm, string _session_to)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>(), new XenRefConverter<Session>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.VM.assert_can_be_recovered", new JArray(session, _vm ?? "", _session_to ?? ""), serializer);
        }

        public List<XenRef<SR>> vm_get_srs_required_for_recovery(string session, string _vm, string _session_to)
        {
            var converters = new List<JsonConverter> {new XenRefListConverter<SR>(), new XenRefConverter<Session>()};
            var serializer = CreateSerializer(converters);
            return Rpc<List<XenRef<SR>>>("VM.get_SRs_required_for_recovery", new JArray(session, _vm ?? "", _session_to ?? ""), serializer);
        }

        public XenRef<Task> async_vm_get_srs_required_for_recovery(string session, string _vm, string _session_to)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>(), new XenRefConverter<Session>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.VM.get_SRs_required_for_recovery", new JArray(session, _vm ?? "", _session_to ?? ""), serializer);
        }

        public void vm_recover(string session, string _vm, string _session_to, bool _force)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Session>()};
            var serializer = CreateSerializer(converters);
            Rpc("VM.recover", new JArray(session, _vm ?? "", _session_to ?? "", _force), serializer);
        }

        public XenRef<Task> async_vm_recover(string session, string _vm, string _session_to, bool _force)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>(), new XenRefConverter<Session>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.VM.recover", new JArray(session, _vm ?? "", _session_to ?? "", _force), serializer);
        }

        public void vm_import_convert(string session, string _type, string _username, string _password, string _sr, Dictionary<string, string> _remote_config)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<SR>(), new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            Rpc("VM.import_convert", new JArray(session, _type ?? "", _username ?? "", _password ?? "", _sr ?? "", _remote_config == null ? new JObject() : JObject.FromObject(_remote_config, serializer)), serializer);
        }

        public XenRef<Task> async_vm_import_convert(string session, string _type, string _username, string _password, string _sr, Dictionary<string, string> _remote_config)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>(), new XenRefConverter<SR>(), new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.VM.import_convert", new JArray(session, _type ?? "", _username ?? "", _password ?? "", _sr ?? "", _remote_config == null ? new JObject() : JObject.FromObject(_remote_config, serializer)), serializer);
        }

        public void vm_set_appliance(string session, string _vm, string _value)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<VM_appliance>()};
            var serializer = CreateSerializer(converters);
            Rpc("VM.set_appliance", new JArray(session, _vm ?? "", _value ?? ""), serializer);
        }

        public XenRef<Task> async_vm_set_appliance(string session, string _vm, string _value)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>(), new XenRefConverter<VM_appliance>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.VM.set_appliance", new JArray(session, _vm ?? "", _value ?? ""), serializer);
        }

        public Dictionary<string, string> vm_query_services(string session, string _vm)
        {
            var converters = new List<JsonConverter> {new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<string, string>>("VM.query_services", new JArray(session, _vm ?? ""), serializer);
        }

        public XenRef<Task> async_vm_query_services(string session, string _vm)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.VM.query_services", new JArray(session, _vm ?? ""), serializer);
        }

        public string vm_call_plugin(string session, string _vm, string _plugin, string _fn, Dictionary<string, string> _args)
        {
            var converters = new List<JsonConverter> {new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("VM.call_plugin", new JArray(session, _vm ?? "", _plugin ?? "", _fn ?? "", _args == null ? new JObject() : JObject.FromObject(_args, serializer)), serializer);
        }

        public XenRef<Task> async_vm_call_plugin(string session, string _vm, string _plugin, string _fn, Dictionary<string, string> _args)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>(), new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.VM.call_plugin", new JArray(session, _vm ?? "", _plugin ?? "", _fn ?? "", _args == null ? new JObject() : JObject.FromObject(_args, serializer)), serializer);
        }

        public void vm_set_has_vendor_device(string session, string _vm, bool _value)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("VM.set_has_vendor_device", new JArray(session, _vm ?? "", _value), serializer);
        }

        public XenRef<Task> async_vm_set_has_vendor_device(string session, string _vm, bool _value)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.VM.set_has_vendor_device", new JArray(session, _vm ?? "", _value), serializer);
        }

        public List<XenRef<VM>> vm_import(string session, string _url, string _sr, bool _full_restore, bool _force)
        {
            var converters = new List<JsonConverter> {new XenRefListConverter<VM>(), new XenRefConverter<SR>()};
            var serializer = CreateSerializer(converters);
            return Rpc<List<XenRef<VM>>>("VM.import", new JArray(session, _url ?? "", _sr ?? "", _full_restore, _force), serializer);
        }

        public XenRef<Task> async_vm_import(string session, string _url, string _sr, bool _full_restore, bool _force)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>(), new XenRefConverter<SR>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.VM.import", new JArray(session, _url ?? "", _sr ?? "", _full_restore, _force), serializer);
        }

        public void vm_set_actions_after_crash(string session, string _vm, on_crash_behaviour _value)
        {
            var converters = new List<JsonConverter> {new on_crash_behaviourConverter()};
            var serializer = CreateSerializer(converters);
            Rpc("VM.set_actions_after_crash", new JArray(session, _vm ?? "", _value.StringOf()), serializer);
        }

        public XenRef<Task> async_vm_set_actions_after_crash(string session, string _vm, on_crash_behaviour _value)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>(), new on_crash_behaviourConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.VM.set_actions_after_crash", new JArray(session, _vm ?? "", _value.StringOf()), serializer);
        }

        public void vm_set_domain_type(string session, string _vm, domain_type _value)
        {
            var converters = new List<JsonConverter> {new domain_typeConverter()};
            var serializer = CreateSerializer(converters);
            Rpc("VM.set_domain_type", new JArray(session, _vm ?? "", _value.StringOf()), serializer);
        }

        public void vm_set_hvm_boot_policy(string session, string _vm, string _value)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("VM.set_HVM_boot_policy", new JArray(session, _vm ?? "", _value ?? ""), serializer);
        }

        public List<XenRef<VM>> vm_get_all(string session)
        {
            var converters = new List<JsonConverter> {new XenRefListConverter<VM>()};
            var serializer = CreateSerializer(converters);
            return Rpc<List<XenRef<VM>>>("VM.get_all", new JArray(session), serializer);
        }

        public Dictionary<XenRef<VM>, VM> vm_get_all_records(string session)
        {
            var converters = new List<JsonConverter> {new XenRefXenObjectMapConverter<VM>()};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<XenRef<VM>, VM>>("VM.get_all_records", new JArray(session), serializer);
        }

        public VM_metrics vm_metrics_get_record(string session, string _vm_metrics)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<VM_metrics>("VM_metrics.get_record", new JArray(session, _vm_metrics ?? ""), serializer);
        }

        public XenRef<VM_metrics> vm_metrics_get_by_uuid(string session, string _uuid)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<VM_metrics>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<VM_metrics>>("VM_metrics.get_by_uuid", new JArray(session, _uuid ?? ""), serializer);
        }

        public string vm_metrics_get_uuid(string session, string _vm_metrics)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("VM_metrics.get_uuid", new JArray(session, _vm_metrics ?? ""), serializer);
        }

        public long vm_metrics_get_memory_actual(string session, string _vm_metrics)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<long>("VM_metrics.get_memory_actual", new JArray(session, _vm_metrics ?? ""), serializer);
        }

        public long vm_metrics_get_vcpus_number(string session, string _vm_metrics)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<long>("VM_metrics.get_VCPUs_number", new JArray(session, _vm_metrics ?? ""), serializer);
        }

        public Dictionary<long, double> vm_metrics_get_vcpus_utilisation(string session, string _vm_metrics)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<long, double>>("VM_metrics.get_VCPUs_utilisation", new JArray(session, _vm_metrics ?? ""), serializer);
        }

        public Dictionary<long, long> vm_metrics_get_vcpus_cpu(string session, string _vm_metrics)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<long, long>>("VM_metrics.get_VCPUs_CPU", new JArray(session, _vm_metrics ?? ""), serializer);
        }

        public Dictionary<string, string> vm_metrics_get_vcpus_params(string session, string _vm_metrics)
        {
            var converters = new List<JsonConverter> {new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<string, string>>("VM_metrics.get_VCPUs_params", new JArray(session, _vm_metrics ?? ""), serializer);
        }

        public Dictionary<long, string[]> vm_metrics_get_vcpus_flags(string session, string _vm_metrics)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<long, string[]>>("VM_metrics.get_VCPUs_flags", new JArray(session, _vm_metrics ?? ""), serializer);
        }

        public string[] vm_metrics_get_state(string session, string _vm_metrics)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string[]>("VM_metrics.get_state", new JArray(session, _vm_metrics ?? ""), serializer);
        }

        public DateTime vm_metrics_get_start_time(string session, string _vm_metrics)
        {
            var converters = new List<JsonConverter> {new XenDateTimeConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<DateTime>("VM_metrics.get_start_time", new JArray(session, _vm_metrics ?? ""), serializer);
        }

        public DateTime vm_metrics_get_install_time(string session, string _vm_metrics)
        {
            var converters = new List<JsonConverter> {new XenDateTimeConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<DateTime>("VM_metrics.get_install_time", new JArray(session, _vm_metrics ?? ""), serializer);
        }

        public DateTime vm_metrics_get_last_updated(string session, string _vm_metrics)
        {
            var converters = new List<JsonConverter> {new XenDateTimeConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<DateTime>("VM_metrics.get_last_updated", new JArray(session, _vm_metrics ?? ""), serializer);
        }

        public Dictionary<string, string> vm_metrics_get_other_config(string session, string _vm_metrics)
        {
            var converters = new List<JsonConverter> {new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<string, string>>("VM_metrics.get_other_config", new JArray(session, _vm_metrics ?? ""), serializer);
        }

        public bool vm_metrics_get_hvm(string session, string _vm_metrics)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<bool>("VM_metrics.get_hvm", new JArray(session, _vm_metrics ?? ""), serializer);
        }

        public bool vm_metrics_get_nested_virt(string session, string _vm_metrics)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<bool>("VM_metrics.get_nested_virt", new JArray(session, _vm_metrics ?? ""), serializer);
        }

        public bool vm_metrics_get_nomigrate(string session, string _vm_metrics)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<bool>("VM_metrics.get_nomigrate", new JArray(session, _vm_metrics ?? ""), serializer);
        }

        public domain_type vm_metrics_get_current_domain_type(string session, string _vm_metrics)
        {
            var converters = new List<JsonConverter> {new domain_typeConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<domain_type>("VM_metrics.get_current_domain_type", new JArray(session, _vm_metrics ?? ""), serializer);
        }

        public void vm_metrics_set_other_config(string session, string _vm_metrics, Dictionary<string, string> _other_config)
        {
            var converters = new List<JsonConverter> {new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            Rpc("VM_metrics.set_other_config", new JArray(session, _vm_metrics ?? "", _other_config == null ? new JObject() : JObject.FromObject(_other_config, serializer)), serializer);
        }

        public void vm_metrics_add_to_other_config(string session, string _vm_metrics, string _key, string _value)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("VM_metrics.add_to_other_config", new JArray(session, _vm_metrics ?? "", _key ?? "", _value ?? ""), serializer);
        }

        public void vm_metrics_remove_from_other_config(string session, string _vm_metrics, string _key)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("VM_metrics.remove_from_other_config", new JArray(session, _vm_metrics ?? "", _key ?? ""), serializer);
        }

        public List<XenRef<VM_metrics>> vm_metrics_get_all(string session)
        {
            var converters = new List<JsonConverter> {new XenRefListConverter<VM_metrics>()};
            var serializer = CreateSerializer(converters);
            return Rpc<List<XenRef<VM_metrics>>>("VM_metrics.get_all", new JArray(session), serializer);
        }

        public Dictionary<XenRef<VM_metrics>, VM_metrics> vm_metrics_get_all_records(string session)
        {
            var converters = new List<JsonConverter> {new XenRefXenObjectMapConverter<VM_metrics>()};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<XenRef<VM_metrics>, VM_metrics>>("VM_metrics.get_all_records", new JArray(session), serializer);
        }

        public VM_guest_metrics vm_guest_metrics_get_record(string session, string _vm_guest_metrics)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<VM_guest_metrics>("VM_guest_metrics.get_record", new JArray(session, _vm_guest_metrics ?? ""), serializer);
        }

        public XenRef<VM_guest_metrics> vm_guest_metrics_get_by_uuid(string session, string _uuid)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<VM_guest_metrics>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<VM_guest_metrics>>("VM_guest_metrics.get_by_uuid", new JArray(session, _uuid ?? ""), serializer);
        }

        public string vm_guest_metrics_get_uuid(string session, string _vm_guest_metrics)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("VM_guest_metrics.get_uuid", new JArray(session, _vm_guest_metrics ?? ""), serializer);
        }

        public Dictionary<string, string> vm_guest_metrics_get_os_version(string session, string _vm_guest_metrics)
        {
            var converters = new List<JsonConverter> {new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<string, string>>("VM_guest_metrics.get_os_version", new JArray(session, _vm_guest_metrics ?? ""), serializer);
        }

        public Dictionary<string, string> vm_guest_metrics_get_pv_drivers_version(string session, string _vm_guest_metrics)
        {
            var converters = new List<JsonConverter> {new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<string, string>>("VM_guest_metrics.get_PV_drivers_version", new JArray(session, _vm_guest_metrics ?? ""), serializer);
        }

        public bool vm_guest_metrics_get_pv_drivers_up_to_date(string session, string _vm_guest_metrics)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<bool>("VM_guest_metrics.get_PV_drivers_up_to_date", new JArray(session, _vm_guest_metrics ?? ""), serializer);
        }

        public Dictionary<string, string> vm_guest_metrics_get_memory(string session, string _vm_guest_metrics)
        {
            var converters = new List<JsonConverter> {new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<string, string>>("VM_guest_metrics.get_memory", new JArray(session, _vm_guest_metrics ?? ""), serializer);
        }

        public Dictionary<string, string> vm_guest_metrics_get_disks(string session, string _vm_guest_metrics)
        {
            var converters = new List<JsonConverter> {new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<string, string>>("VM_guest_metrics.get_disks", new JArray(session, _vm_guest_metrics ?? ""), serializer);
        }

        public Dictionary<string, string> vm_guest_metrics_get_networks(string session, string _vm_guest_metrics)
        {
            var converters = new List<JsonConverter> {new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<string, string>>("VM_guest_metrics.get_networks", new JArray(session, _vm_guest_metrics ?? ""), serializer);
        }

        public Dictionary<string, string> vm_guest_metrics_get_other(string session, string _vm_guest_metrics)
        {
            var converters = new List<JsonConverter> {new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<string, string>>("VM_guest_metrics.get_other", new JArray(session, _vm_guest_metrics ?? ""), serializer);
        }

        public DateTime vm_guest_metrics_get_last_updated(string session, string _vm_guest_metrics)
        {
            var converters = new List<JsonConverter> {new XenDateTimeConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<DateTime>("VM_guest_metrics.get_last_updated", new JArray(session, _vm_guest_metrics ?? ""), serializer);
        }

        public Dictionary<string, string> vm_guest_metrics_get_other_config(string session, string _vm_guest_metrics)
        {
            var converters = new List<JsonConverter> {new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<string, string>>("VM_guest_metrics.get_other_config", new JArray(session, _vm_guest_metrics ?? ""), serializer);
        }

        public bool vm_guest_metrics_get_live(string session, string _vm_guest_metrics)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<bool>("VM_guest_metrics.get_live", new JArray(session, _vm_guest_metrics ?? ""), serializer);
        }

        public tristate_type vm_guest_metrics_get_can_use_hotplug_vbd(string session, string _vm_guest_metrics)
        {
            var converters = new List<JsonConverter> {new tristate_typeConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<tristate_type>("VM_guest_metrics.get_can_use_hotplug_vbd", new JArray(session, _vm_guest_metrics ?? ""), serializer);
        }

        public tristate_type vm_guest_metrics_get_can_use_hotplug_vif(string session, string _vm_guest_metrics)
        {
            var converters = new List<JsonConverter> {new tristate_typeConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<tristate_type>("VM_guest_metrics.get_can_use_hotplug_vif", new JArray(session, _vm_guest_metrics ?? ""), serializer);
        }

        public bool vm_guest_metrics_get_pv_drivers_detected(string session, string _vm_guest_metrics)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<bool>("VM_guest_metrics.get_PV_drivers_detected", new JArray(session, _vm_guest_metrics ?? ""), serializer);
        }

        public void vm_guest_metrics_set_other_config(string session, string _vm_guest_metrics, Dictionary<string, string> _other_config)
        {
            var converters = new List<JsonConverter> {new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            Rpc("VM_guest_metrics.set_other_config", new JArray(session, _vm_guest_metrics ?? "", _other_config == null ? new JObject() : JObject.FromObject(_other_config, serializer)), serializer);
        }

        public void vm_guest_metrics_add_to_other_config(string session, string _vm_guest_metrics, string _key, string _value)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("VM_guest_metrics.add_to_other_config", new JArray(session, _vm_guest_metrics ?? "", _key ?? "", _value ?? ""), serializer);
        }

        public void vm_guest_metrics_remove_from_other_config(string session, string _vm_guest_metrics, string _key)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("VM_guest_metrics.remove_from_other_config", new JArray(session, _vm_guest_metrics ?? "", _key ?? ""), serializer);
        }

        public List<XenRef<VM_guest_metrics>> vm_guest_metrics_get_all(string session)
        {
            var converters = new List<JsonConverter> {new XenRefListConverter<VM_guest_metrics>()};
            var serializer = CreateSerializer(converters);
            return Rpc<List<XenRef<VM_guest_metrics>>>("VM_guest_metrics.get_all", new JArray(session), serializer);
        }

        public Dictionary<XenRef<VM_guest_metrics>, VM_guest_metrics> vm_guest_metrics_get_all_records(string session)
        {
            var converters = new List<JsonConverter> {new XenRefXenObjectMapConverter<VM_guest_metrics>()};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<XenRef<VM_guest_metrics>, VM_guest_metrics>>("VM_guest_metrics.get_all_records", new JArray(session), serializer);
        }

        public VMPP vmpp_get_record(string session, string _vmpp)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<VMPP>("VMPP.get_record", new JArray(session, _vmpp ?? ""), serializer);
        }

        public XenRef<VMPP> vmpp_get_by_uuid(string session, string _uuid)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<VMPP>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<VMPP>>("VMPP.get_by_uuid", new JArray(session, _uuid ?? ""), serializer);
        }

        public XenRef<VMPP> vmpp_create(string session, VMPP _record)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<VMPP>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<VMPP>>("VMPP.create", new JArray(session, _record.ToJObject()), serializer);
        }

        public XenRef<Task> async_vmpp_create(string session, VMPP _record)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.VMPP.create", new JArray(session, _record.ToJObject()), serializer);
        }

        public void vmpp_destroy(string session, string _vmpp)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("VMPP.destroy", new JArray(session, _vmpp ?? ""), serializer);
        }

        public XenRef<Task> async_vmpp_destroy(string session, string _vmpp)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.VMPP.destroy", new JArray(session, _vmpp ?? ""), serializer);
        }

        public List<XenRef<VMPP>> vmpp_get_by_name_label(string session, string _label)
        {
            var converters = new List<JsonConverter> {new XenRefListConverter<VMPP>()};
            var serializer = CreateSerializer(converters);
            return Rpc<List<XenRef<VMPP>>>("VMPP.get_by_name_label", new JArray(session, _label ?? ""), serializer);
        }

        public string vmpp_get_uuid(string session, string _vmpp)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("VMPP.get_uuid", new JArray(session, _vmpp ?? ""), serializer);
        }

        public string vmpp_get_name_label(string session, string _vmpp)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("VMPP.get_name_label", new JArray(session, _vmpp ?? ""), serializer);
        }

        public string vmpp_get_name_description(string session, string _vmpp)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("VMPP.get_name_description", new JArray(session, _vmpp ?? ""), serializer);
        }

        public bool vmpp_get_is_policy_enabled(string session, string _vmpp)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<bool>("VMPP.get_is_policy_enabled", new JArray(session, _vmpp ?? ""), serializer);
        }

        public vmpp_backup_type vmpp_get_backup_type(string session, string _vmpp)
        {
            var converters = new List<JsonConverter> {new vmpp_backup_typeConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<vmpp_backup_type>("VMPP.get_backup_type", new JArray(session, _vmpp ?? ""), serializer);
        }

        public long vmpp_get_backup_retention_value(string session, string _vmpp)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<long>("VMPP.get_backup_retention_value", new JArray(session, _vmpp ?? ""), serializer);
        }

        public vmpp_backup_frequency vmpp_get_backup_frequency(string session, string _vmpp)
        {
            var converters = new List<JsonConverter> {new vmpp_backup_frequencyConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<vmpp_backup_frequency>("VMPP.get_backup_frequency", new JArray(session, _vmpp ?? ""), serializer);
        }

        public Dictionary<string, string> vmpp_get_backup_schedule(string session, string _vmpp)
        {
            var converters = new List<JsonConverter> {new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<string, string>>("VMPP.get_backup_schedule", new JArray(session, _vmpp ?? ""), serializer);
        }

        public bool vmpp_get_is_backup_running(string session, string _vmpp)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<bool>("VMPP.get_is_backup_running", new JArray(session, _vmpp ?? ""), serializer);
        }

        public DateTime vmpp_get_backup_last_run_time(string session, string _vmpp)
        {
            var converters = new List<JsonConverter> {new XenDateTimeConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<DateTime>("VMPP.get_backup_last_run_time", new JArray(session, _vmpp ?? ""), serializer);
        }

        public vmpp_archive_target_type vmpp_get_archive_target_type(string session, string _vmpp)
        {
            var converters = new List<JsonConverter> {new vmpp_archive_target_typeConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<vmpp_archive_target_type>("VMPP.get_archive_target_type", new JArray(session, _vmpp ?? ""), serializer);
        }

        public Dictionary<string, string> vmpp_get_archive_target_config(string session, string _vmpp)
        {
            var converters = new List<JsonConverter> {new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<string, string>>("VMPP.get_archive_target_config", new JArray(session, _vmpp ?? ""), serializer);
        }

        public vmpp_archive_frequency vmpp_get_archive_frequency(string session, string _vmpp)
        {
            var converters = new List<JsonConverter> {new vmpp_archive_frequencyConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<vmpp_archive_frequency>("VMPP.get_archive_frequency", new JArray(session, _vmpp ?? ""), serializer);
        }

        public Dictionary<string, string> vmpp_get_archive_schedule(string session, string _vmpp)
        {
            var converters = new List<JsonConverter> {new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<string, string>>("VMPP.get_archive_schedule", new JArray(session, _vmpp ?? ""), serializer);
        }

        public bool vmpp_get_is_archive_running(string session, string _vmpp)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<bool>("VMPP.get_is_archive_running", new JArray(session, _vmpp ?? ""), serializer);
        }

        public DateTime vmpp_get_archive_last_run_time(string session, string _vmpp)
        {
            var converters = new List<JsonConverter> {new XenDateTimeConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<DateTime>("VMPP.get_archive_last_run_time", new JArray(session, _vmpp ?? ""), serializer);
        }

        public List<XenRef<VM>> vmpp_get_vms(string session, string _vmpp)
        {
            var converters = new List<JsonConverter> {new XenRefListConverter<VM>()};
            var serializer = CreateSerializer(converters);
            return Rpc<List<XenRef<VM>>>("VMPP.get_VMs", new JArray(session, _vmpp ?? ""), serializer);
        }

        public bool vmpp_get_is_alarm_enabled(string session, string _vmpp)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<bool>("VMPP.get_is_alarm_enabled", new JArray(session, _vmpp ?? ""), serializer);
        }

        public Dictionary<string, string> vmpp_get_alarm_config(string session, string _vmpp)
        {
            var converters = new List<JsonConverter> {new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<string, string>>("VMPP.get_alarm_config", new JArray(session, _vmpp ?? ""), serializer);
        }

        public string[] vmpp_get_recent_alerts(string session, string _vmpp)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string[]>("VMPP.get_recent_alerts", new JArray(session, _vmpp ?? ""), serializer);
        }

        public void vmpp_set_name_label(string session, string _vmpp, string _label)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("VMPP.set_name_label", new JArray(session, _vmpp ?? "", _label ?? ""), serializer);
        }

        public void vmpp_set_name_description(string session, string _vmpp, string _description)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("VMPP.set_name_description", new JArray(session, _vmpp ?? "", _description ?? ""), serializer);
        }

        public void vmpp_set_is_policy_enabled(string session, string _vmpp, bool _is_policy_enabled)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("VMPP.set_is_policy_enabled", new JArray(session, _vmpp ?? "", _is_policy_enabled), serializer);
        }

        public void vmpp_set_backup_type(string session, string _vmpp, vmpp_backup_type _backup_type)
        {
            var converters = new List<JsonConverter> {new vmpp_backup_typeConverter()};
            var serializer = CreateSerializer(converters);
            Rpc("VMPP.set_backup_type", new JArray(session, _vmpp ?? "", _backup_type.StringOf()), serializer);
        }

        public string vmpp_protect_now(string session, string _vmpp)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("VMPP.protect_now", new JArray(session, _vmpp ?? ""), serializer);
        }

        public string vmpp_archive_now(string session, string _snapshot)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<VM>()};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("VMPP.archive_now", new JArray(session, _snapshot ?? ""), serializer);
        }

        public string[] vmpp_get_alerts(string session, string _vmpp, long _hours_from_now)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string[]>("VMPP.get_alerts", new JArray(session, _vmpp ?? "", _hours_from_now), serializer);
        }

        public void vmpp_set_backup_retention_value(string session, string _vmpp, long _value)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("VMPP.set_backup_retention_value", new JArray(session, _vmpp ?? "", _value), serializer);
        }

        public void vmpp_set_backup_frequency(string session, string _vmpp, vmpp_backup_frequency _value)
        {
            var converters = new List<JsonConverter> {new vmpp_backup_frequencyConverter()};
            var serializer = CreateSerializer(converters);
            Rpc("VMPP.set_backup_frequency", new JArray(session, _vmpp ?? "", _value.StringOf()), serializer);
        }

        public void vmpp_set_backup_schedule(string session, string _vmpp, Dictionary<string, string> _value)
        {
            var converters = new List<JsonConverter> {new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            Rpc("VMPP.set_backup_schedule", new JArray(session, _vmpp ?? "", _value == null ? new JObject() : JObject.FromObject(_value, serializer)), serializer);
        }

        public void vmpp_set_archive_frequency(string session, string _vmpp, vmpp_archive_frequency _value)
        {
            var converters = new List<JsonConverter> {new vmpp_archive_frequencyConverter()};
            var serializer = CreateSerializer(converters);
            Rpc("VMPP.set_archive_frequency", new JArray(session, _vmpp ?? "", _value.StringOf()), serializer);
        }

        public void vmpp_set_archive_schedule(string session, string _vmpp, Dictionary<string, string> _value)
        {
            var converters = new List<JsonConverter> {new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            Rpc("VMPP.set_archive_schedule", new JArray(session, _vmpp ?? "", _value == null ? new JObject() : JObject.FromObject(_value, serializer)), serializer);
        }

        public void vmpp_set_archive_target_type(string session, string _vmpp, vmpp_archive_target_type _value)
        {
            var converters = new List<JsonConverter> {new vmpp_archive_target_typeConverter()};
            var serializer = CreateSerializer(converters);
            Rpc("VMPP.set_archive_target_type", new JArray(session, _vmpp ?? "", _value.StringOf()), serializer);
        }

        public void vmpp_set_archive_target_config(string session, string _vmpp, Dictionary<string, string> _value)
        {
            var converters = new List<JsonConverter> {new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            Rpc("VMPP.set_archive_target_config", new JArray(session, _vmpp ?? "", _value == null ? new JObject() : JObject.FromObject(_value, serializer)), serializer);
        }

        public void vmpp_set_is_alarm_enabled(string session, string _vmpp, bool _value)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("VMPP.set_is_alarm_enabled", new JArray(session, _vmpp ?? "", _value), serializer);
        }

        public void vmpp_set_alarm_config(string session, string _vmpp, Dictionary<string, string> _value)
        {
            var converters = new List<JsonConverter> {new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            Rpc("VMPP.set_alarm_config", new JArray(session, _vmpp ?? "", _value == null ? new JObject() : JObject.FromObject(_value, serializer)), serializer);
        }

        public void vmpp_add_to_backup_schedule(string session, string _vmpp, string _key, string _value)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("VMPP.add_to_backup_schedule", new JArray(session, _vmpp ?? "", _key ?? "", _value ?? ""), serializer);
        }

        public void vmpp_add_to_archive_target_config(string session, string _vmpp, string _key, string _value)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("VMPP.add_to_archive_target_config", new JArray(session, _vmpp ?? "", _key ?? "", _value ?? ""), serializer);
        }

        public void vmpp_add_to_archive_schedule(string session, string _vmpp, string _key, string _value)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("VMPP.add_to_archive_schedule", new JArray(session, _vmpp ?? "", _key ?? "", _value ?? ""), serializer);
        }

        public void vmpp_add_to_alarm_config(string session, string _vmpp, string _key, string _value)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("VMPP.add_to_alarm_config", new JArray(session, _vmpp ?? "", _key ?? "", _value ?? ""), serializer);
        }

        public void vmpp_remove_from_backup_schedule(string session, string _vmpp, string _key)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("VMPP.remove_from_backup_schedule", new JArray(session, _vmpp ?? "", _key ?? ""), serializer);
        }

        public void vmpp_remove_from_archive_target_config(string session, string _vmpp, string _key)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("VMPP.remove_from_archive_target_config", new JArray(session, _vmpp ?? "", _key ?? ""), serializer);
        }

        public void vmpp_remove_from_archive_schedule(string session, string _vmpp, string _key)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("VMPP.remove_from_archive_schedule", new JArray(session, _vmpp ?? "", _key ?? ""), serializer);
        }

        public void vmpp_remove_from_alarm_config(string session, string _vmpp, string _key)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("VMPP.remove_from_alarm_config", new JArray(session, _vmpp ?? "", _key ?? ""), serializer);
        }

        public void vmpp_set_backup_last_run_time(string session, string _vmpp, DateTime _value)
        {
            var converters = new List<JsonConverter> {new XenDateTimeConverter()};
            var serializer = CreateSerializer(converters);
            Rpc("VMPP.set_backup_last_run_time", new JArray(session, _vmpp ?? "", _value), serializer);
        }

        public void vmpp_set_archive_last_run_time(string session, string _vmpp, DateTime _value)
        {
            var converters = new List<JsonConverter> {new XenDateTimeConverter()};
            var serializer = CreateSerializer(converters);
            Rpc("VMPP.set_archive_last_run_time", new JArray(session, _vmpp ?? "", _value), serializer);
        }

        public List<XenRef<VMPP>> vmpp_get_all(string session)
        {
            var converters = new List<JsonConverter> {new XenRefListConverter<VMPP>()};
            var serializer = CreateSerializer(converters);
            return Rpc<List<XenRef<VMPP>>>("VMPP.get_all", new JArray(session), serializer);
        }

        public Dictionary<XenRef<VMPP>, VMPP> vmpp_get_all_records(string session)
        {
            var converters = new List<JsonConverter> {new XenRefXenObjectMapConverter<VMPP>()};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<XenRef<VMPP>, VMPP>>("VMPP.get_all_records", new JArray(session), serializer);
        }

        public VMSS vmss_get_record(string session, string _vmss)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<VMSS>("VMSS.get_record", new JArray(session, _vmss ?? ""), serializer);
        }

        public XenRef<VMSS> vmss_get_by_uuid(string session, string _uuid)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<VMSS>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<VMSS>>("VMSS.get_by_uuid", new JArray(session, _uuid ?? ""), serializer);
        }

        public XenRef<VMSS> vmss_create(string session, VMSS _record)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<VMSS>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<VMSS>>("VMSS.create", new JArray(session, _record.ToJObject()), serializer);
        }

        public XenRef<Task> async_vmss_create(string session, VMSS _record)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.VMSS.create", new JArray(session, _record.ToJObject()), serializer);
        }

        public void vmss_destroy(string session, string _vmss)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("VMSS.destroy", new JArray(session, _vmss ?? ""), serializer);
        }

        public XenRef<Task> async_vmss_destroy(string session, string _vmss)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.VMSS.destroy", new JArray(session, _vmss ?? ""), serializer);
        }

        public List<XenRef<VMSS>> vmss_get_by_name_label(string session, string _label)
        {
            var converters = new List<JsonConverter> {new XenRefListConverter<VMSS>()};
            var serializer = CreateSerializer(converters);
            return Rpc<List<XenRef<VMSS>>>("VMSS.get_by_name_label", new JArray(session, _label ?? ""), serializer);
        }

        public string vmss_get_uuid(string session, string _vmss)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("VMSS.get_uuid", new JArray(session, _vmss ?? ""), serializer);
        }

        public string vmss_get_name_label(string session, string _vmss)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("VMSS.get_name_label", new JArray(session, _vmss ?? ""), serializer);
        }

        public string vmss_get_name_description(string session, string _vmss)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("VMSS.get_name_description", new JArray(session, _vmss ?? ""), serializer);
        }

        public bool vmss_get_enabled(string session, string _vmss)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<bool>("VMSS.get_enabled", new JArray(session, _vmss ?? ""), serializer);
        }

        public vmss_type vmss_get_type(string session, string _vmss)
        {
            var converters = new List<JsonConverter> {new vmss_typeConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<vmss_type>("VMSS.get_type", new JArray(session, _vmss ?? ""), serializer);
        }

        public long vmss_get_retained_snapshots(string session, string _vmss)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<long>("VMSS.get_retained_snapshots", new JArray(session, _vmss ?? ""), serializer);
        }

        public vmss_frequency vmss_get_frequency(string session, string _vmss)
        {
            var converters = new List<JsonConverter> {new vmss_frequencyConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<vmss_frequency>("VMSS.get_frequency", new JArray(session, _vmss ?? ""), serializer);
        }

        public Dictionary<string, string> vmss_get_schedule(string session, string _vmss)
        {
            var converters = new List<JsonConverter> {new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<string, string>>("VMSS.get_schedule", new JArray(session, _vmss ?? ""), serializer);
        }

        public DateTime vmss_get_last_run_time(string session, string _vmss)
        {
            var converters = new List<JsonConverter> {new XenDateTimeConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<DateTime>("VMSS.get_last_run_time", new JArray(session, _vmss ?? ""), serializer);
        }

        public List<XenRef<VM>> vmss_get_vms(string session, string _vmss)
        {
            var converters = new List<JsonConverter> {new XenRefListConverter<VM>()};
            var serializer = CreateSerializer(converters);
            return Rpc<List<XenRef<VM>>>("VMSS.get_VMs", new JArray(session, _vmss ?? ""), serializer);
        }

        public void vmss_set_name_label(string session, string _vmss, string _label)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("VMSS.set_name_label", new JArray(session, _vmss ?? "", _label ?? ""), serializer);
        }

        public void vmss_set_name_description(string session, string _vmss, string _description)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("VMSS.set_name_description", new JArray(session, _vmss ?? "", _description ?? ""), serializer);
        }

        public void vmss_set_enabled(string session, string _vmss, bool _enabled)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("VMSS.set_enabled", new JArray(session, _vmss ?? "", _enabled), serializer);
        }

        public string vmss_snapshot_now(string session, string _vmss)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("VMSS.snapshot_now", new JArray(session, _vmss ?? ""), serializer);
        }

        public void vmss_set_retained_snapshots(string session, string _vmss, long _value)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("VMSS.set_retained_snapshots", new JArray(session, _vmss ?? "", _value), serializer);
        }

        public void vmss_set_frequency(string session, string _vmss, vmss_frequency _value)
        {
            var converters = new List<JsonConverter> {new vmss_frequencyConverter()};
            var serializer = CreateSerializer(converters);
            Rpc("VMSS.set_frequency", new JArray(session, _vmss ?? "", _value.StringOf()), serializer);
        }

        public void vmss_set_schedule(string session, string _vmss, Dictionary<string, string> _value)
        {
            var converters = new List<JsonConverter> {new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            Rpc("VMSS.set_schedule", new JArray(session, _vmss ?? "", _value == null ? new JObject() : JObject.FromObject(_value, serializer)), serializer);
        }

        public void vmss_add_to_schedule(string session, string _vmss, string _key, string _value)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("VMSS.add_to_schedule", new JArray(session, _vmss ?? "", _key ?? "", _value ?? ""), serializer);
        }

        public void vmss_remove_from_schedule(string session, string _vmss, string _key)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("VMSS.remove_from_schedule", new JArray(session, _vmss ?? "", _key ?? ""), serializer);
        }

        public void vmss_set_last_run_time(string session, string _vmss, DateTime _value)
        {
            var converters = new List<JsonConverter> {new XenDateTimeConverter()};
            var serializer = CreateSerializer(converters);
            Rpc("VMSS.set_last_run_time", new JArray(session, _vmss ?? "", _value), serializer);
        }

        public void vmss_set_type(string session, string _vmss, vmss_type _value)
        {
            var converters = new List<JsonConverter> {new vmss_typeConverter()};
            var serializer = CreateSerializer(converters);
            Rpc("VMSS.set_type", new JArray(session, _vmss ?? "", _value.StringOf()), serializer);
        }

        public List<XenRef<VMSS>> vmss_get_all(string session)
        {
            var converters = new List<JsonConverter> {new XenRefListConverter<VMSS>()};
            var serializer = CreateSerializer(converters);
            return Rpc<List<XenRef<VMSS>>>("VMSS.get_all", new JArray(session), serializer);
        }

        public Dictionary<XenRef<VMSS>, VMSS> vmss_get_all_records(string session)
        {
            var converters = new List<JsonConverter> {new XenRefXenObjectMapConverter<VMSS>()};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<XenRef<VMSS>, VMSS>>("VMSS.get_all_records", new JArray(session), serializer);
        }

        public VM_appliance vm_appliance_get_record(string session, string _vm_appliance)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<VM_appliance>("VM_appliance.get_record", new JArray(session, _vm_appliance ?? ""), serializer);
        }

        public XenRef<VM_appliance> vm_appliance_get_by_uuid(string session, string _uuid)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<VM_appliance>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<VM_appliance>>("VM_appliance.get_by_uuid", new JArray(session, _uuid ?? ""), serializer);
        }

        public XenRef<VM_appliance> vm_appliance_create(string session, VM_appliance _record)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<VM_appliance>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<VM_appliance>>("VM_appliance.create", new JArray(session, _record.ToJObject()), serializer);
        }

        public XenRef<Task> async_vm_appliance_create(string session, VM_appliance _record)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.VM_appliance.create", new JArray(session, _record.ToJObject()), serializer);
        }

        public void vm_appliance_destroy(string session, string _vm_appliance)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("VM_appliance.destroy", new JArray(session, _vm_appliance ?? ""), serializer);
        }

        public XenRef<Task> async_vm_appliance_destroy(string session, string _vm_appliance)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.VM_appliance.destroy", new JArray(session, _vm_appliance ?? ""), serializer);
        }

        public List<XenRef<VM_appliance>> vm_appliance_get_by_name_label(string session, string _label)
        {
            var converters = new List<JsonConverter> {new XenRefListConverter<VM_appliance>()};
            var serializer = CreateSerializer(converters);
            return Rpc<List<XenRef<VM_appliance>>>("VM_appliance.get_by_name_label", new JArray(session, _label ?? ""), serializer);
        }

        public string vm_appliance_get_uuid(string session, string _vm_appliance)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("VM_appliance.get_uuid", new JArray(session, _vm_appliance ?? ""), serializer);
        }

        public string vm_appliance_get_name_label(string session, string _vm_appliance)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("VM_appliance.get_name_label", new JArray(session, _vm_appliance ?? ""), serializer);
        }

        public string vm_appliance_get_name_description(string session, string _vm_appliance)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("VM_appliance.get_name_description", new JArray(session, _vm_appliance ?? ""), serializer);
        }

        public List<vm_appliance_operation> vm_appliance_get_allowed_operations(string session, string _vm_appliance)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<List<vm_appliance_operation>>("VM_appliance.get_allowed_operations", new JArray(session, _vm_appliance ?? ""), serializer);
        }

        public Dictionary<string, vm_appliance_operation> vm_appliance_get_current_operations(string session, string _vm_appliance)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<string, vm_appliance_operation>>("VM_appliance.get_current_operations", new JArray(session, _vm_appliance ?? ""), serializer);
        }

        public List<XenRef<VM>> vm_appliance_get_vms(string session, string _vm_appliance)
        {
            var converters = new List<JsonConverter> {new XenRefListConverter<VM>()};
            var serializer = CreateSerializer(converters);
            return Rpc<List<XenRef<VM>>>("VM_appliance.get_VMs", new JArray(session, _vm_appliance ?? ""), serializer);
        }

        public void vm_appliance_set_name_label(string session, string _vm_appliance, string _label)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("VM_appliance.set_name_label", new JArray(session, _vm_appliance ?? "", _label ?? ""), serializer);
        }

        public void vm_appliance_set_name_description(string session, string _vm_appliance, string _description)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("VM_appliance.set_name_description", new JArray(session, _vm_appliance ?? "", _description ?? ""), serializer);
        }

        public void vm_appliance_start(string session, string _vm_appliance, bool _paused)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("VM_appliance.start", new JArray(session, _vm_appliance ?? "", _paused), serializer);
        }

        public XenRef<Task> async_vm_appliance_start(string session, string _vm_appliance, bool _paused)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.VM_appliance.start", new JArray(session, _vm_appliance ?? "", _paused), serializer);
        }

        public void vm_appliance_clean_shutdown(string session, string _vm_appliance)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("VM_appliance.clean_shutdown", new JArray(session, _vm_appliance ?? ""), serializer);
        }

        public XenRef<Task> async_vm_appliance_clean_shutdown(string session, string _vm_appliance)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.VM_appliance.clean_shutdown", new JArray(session, _vm_appliance ?? ""), serializer);
        }

        public void vm_appliance_hard_shutdown(string session, string _vm_appliance)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("VM_appliance.hard_shutdown", new JArray(session, _vm_appliance ?? ""), serializer);
        }

        public XenRef<Task> async_vm_appliance_hard_shutdown(string session, string _vm_appliance)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.VM_appliance.hard_shutdown", new JArray(session, _vm_appliance ?? ""), serializer);
        }

        public void vm_appliance_shutdown(string session, string _vm_appliance)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("VM_appliance.shutdown", new JArray(session, _vm_appliance ?? ""), serializer);
        }

        public XenRef<Task> async_vm_appliance_shutdown(string session, string _vm_appliance)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.VM_appliance.shutdown", new JArray(session, _vm_appliance ?? ""), serializer);
        }

        public void vm_appliance_assert_can_be_recovered(string session, string _vm_appliance, string _session_to)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Session>()};
            var serializer = CreateSerializer(converters);
            Rpc("VM_appliance.assert_can_be_recovered", new JArray(session, _vm_appliance ?? "", _session_to ?? ""), serializer);
        }

        public XenRef<Task> async_vm_appliance_assert_can_be_recovered(string session, string _vm_appliance, string _session_to)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>(), new XenRefConverter<Session>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.VM_appliance.assert_can_be_recovered", new JArray(session, _vm_appliance ?? "", _session_to ?? ""), serializer);
        }

        public List<XenRef<SR>> vm_appliance_get_srs_required_for_recovery(string session, string _vm_appliance, string _session_to)
        {
            var converters = new List<JsonConverter> {new XenRefListConverter<SR>(), new XenRefConverter<Session>()};
            var serializer = CreateSerializer(converters);
            return Rpc<List<XenRef<SR>>>("VM_appliance.get_SRs_required_for_recovery", new JArray(session, _vm_appliance ?? "", _session_to ?? ""), serializer);
        }

        public XenRef<Task> async_vm_appliance_get_srs_required_for_recovery(string session, string _vm_appliance, string _session_to)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>(), new XenRefConverter<Session>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.VM_appliance.get_SRs_required_for_recovery", new JArray(session, _vm_appliance ?? "", _session_to ?? ""), serializer);
        }

        public void vm_appliance_recover(string session, string _vm_appliance, string _session_to, bool _force)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Session>()};
            var serializer = CreateSerializer(converters);
            Rpc("VM_appliance.recover", new JArray(session, _vm_appliance ?? "", _session_to ?? "", _force), serializer);
        }

        public XenRef<Task> async_vm_appliance_recover(string session, string _vm_appliance, string _session_to, bool _force)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>(), new XenRefConverter<Session>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.VM_appliance.recover", new JArray(session, _vm_appliance ?? "", _session_to ?? "", _force), serializer);
        }

        public List<XenRef<VM_appliance>> vm_appliance_get_all(string session)
        {
            var converters = new List<JsonConverter> {new XenRefListConverter<VM_appliance>()};
            var serializer = CreateSerializer(converters);
            return Rpc<List<XenRef<VM_appliance>>>("VM_appliance.get_all", new JArray(session), serializer);
        }

        public Dictionary<XenRef<VM_appliance>, VM_appliance> vm_appliance_get_all_records(string session)
        {
            var converters = new List<JsonConverter> {new XenRefXenObjectMapConverter<VM_appliance>()};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<XenRef<VM_appliance>, VM_appliance>>("VM_appliance.get_all_records", new JArray(session), serializer);
        }

        public DR_task dr_task_get_record(string session, string _dr_task)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<DR_task>("DR_task.get_record", new JArray(session, _dr_task ?? ""), serializer);
        }

        public XenRef<DR_task> dr_task_get_by_uuid(string session, string _uuid)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<DR_task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<DR_task>>("DR_task.get_by_uuid", new JArray(session, _uuid ?? ""), serializer);
        }

        public string dr_task_get_uuid(string session, string _dr_task)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("DR_task.get_uuid", new JArray(session, _dr_task ?? ""), serializer);
        }

        public List<XenRef<SR>> dr_task_get_introduced_srs(string session, string _dr_task)
        {
            var converters = new List<JsonConverter> {new XenRefListConverter<SR>()};
            var serializer = CreateSerializer(converters);
            return Rpc<List<XenRef<SR>>>("DR_task.get_introduced_SRs", new JArray(session, _dr_task ?? ""), serializer);
        }

        public XenRef<DR_task> dr_task_create(string session, string _type, Dictionary<string, string> _device_config, string[] _whitelist)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<DR_task>(), new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<DR_task>>("DR_task.create", new JArray(session, _type ?? "", _device_config == null ? new JObject() : JObject.FromObject(_device_config, serializer), _whitelist == null ? new JArray() : JArray.FromObject(_whitelist)), serializer);
        }

        public XenRef<Task> async_dr_task_create(string session, string _type, Dictionary<string, string> _device_config, string[] _whitelist)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>(), new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.DR_task.create", new JArray(session, _type ?? "", _device_config == null ? new JObject() : JObject.FromObject(_device_config, serializer), _whitelist == null ? new JArray() : JArray.FromObject(_whitelist)), serializer);
        }

        public void dr_task_destroy(string session, string _dr_task)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("DR_task.destroy", new JArray(session, _dr_task ?? ""), serializer);
        }

        public XenRef<Task> async_dr_task_destroy(string session, string _dr_task)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.DR_task.destroy", new JArray(session, _dr_task ?? ""), serializer);
        }

        public List<XenRef<DR_task>> dr_task_get_all(string session)
        {
            var converters = new List<JsonConverter> {new XenRefListConverter<DR_task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<List<XenRef<DR_task>>>("DR_task.get_all", new JArray(session), serializer);
        }

        public Dictionary<XenRef<DR_task>, DR_task> dr_task_get_all_records(string session)
        {
            var converters = new List<JsonConverter> {new XenRefXenObjectMapConverter<DR_task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<XenRef<DR_task>, DR_task>>("DR_task.get_all_records", new JArray(session), serializer);
        }

        public Host host_get_record(string session, string _host)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<Host>("host.get_record", new JArray(session, _host ?? ""), serializer);
        }

        public XenRef<Host> host_get_by_uuid(string session, string _uuid)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Host>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Host>>("host.get_by_uuid", new JArray(session, _uuid ?? ""), serializer);
        }

        public List<XenRef<Host>> host_get_by_name_label(string session, string _label)
        {
            var converters = new List<JsonConverter> {new XenRefListConverter<Host>()};
            var serializer = CreateSerializer(converters);
            return Rpc<List<XenRef<Host>>>("host.get_by_name_label", new JArray(session, _label ?? ""), serializer);
        }

        public string host_get_uuid(string session, string _host)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("host.get_uuid", new JArray(session, _host ?? ""), serializer);
        }

        public string host_get_name_label(string session, string _host)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("host.get_name_label", new JArray(session, _host ?? ""), serializer);
        }

        public string host_get_name_description(string session, string _host)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("host.get_name_description", new JArray(session, _host ?? ""), serializer);
        }

        public long host_get_memory_overhead(string session, string _host)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<long>("host.get_memory_overhead", new JArray(session, _host ?? ""), serializer);
        }

        public List<host_allowed_operations> host_get_allowed_operations(string session, string _host)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<List<host_allowed_operations>>("host.get_allowed_operations", new JArray(session, _host ?? ""), serializer);
        }

        public Dictionary<string, host_allowed_operations> host_get_current_operations(string session, string _host)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<string, host_allowed_operations>>("host.get_current_operations", new JArray(session, _host ?? ""), serializer);
        }

        public long host_get_api_version_major(string session, string _host)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<long>("host.get_API_version_major", new JArray(session, _host ?? ""), serializer);
        }

        public long host_get_api_version_minor(string session, string _host)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<long>("host.get_API_version_minor", new JArray(session, _host ?? ""), serializer);
        }

        public string host_get_api_version_vendor(string session, string _host)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("host.get_API_version_vendor", new JArray(session, _host ?? ""), serializer);
        }

        public Dictionary<string, string> host_get_api_version_vendor_implementation(string session, string _host)
        {
            var converters = new List<JsonConverter> {new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<string, string>>("host.get_API_version_vendor_implementation", new JArray(session, _host ?? ""), serializer);
        }

        public bool host_get_enabled(string session, string _host)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<bool>("host.get_enabled", new JArray(session, _host ?? ""), serializer);
        }

        public Dictionary<string, string> host_get_software_version(string session, string _host)
        {
            var converters = new List<JsonConverter> {new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<string, string>>("host.get_software_version", new JArray(session, _host ?? ""), serializer);
        }

        public Dictionary<string, string> host_get_other_config(string session, string _host)
        {
            var converters = new List<JsonConverter> {new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<string, string>>("host.get_other_config", new JArray(session, _host ?? ""), serializer);
        }

        public string[] host_get_capabilities(string session, string _host)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string[]>("host.get_capabilities", new JArray(session, _host ?? ""), serializer);
        }

        public Dictionary<string, string> host_get_cpu_configuration(string session, string _host)
        {
            var converters = new List<JsonConverter> {new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<string, string>>("host.get_cpu_configuration", new JArray(session, _host ?? ""), serializer);
        }

        public string host_get_sched_policy(string session, string _host)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("host.get_sched_policy", new JArray(session, _host ?? ""), serializer);
        }

        public string[] host_get_supported_bootloaders(string session, string _host)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string[]>("host.get_supported_bootloaders", new JArray(session, _host ?? ""), serializer);
        }

        public List<XenRef<VM>> host_get_resident_vms(string session, string _host)
        {
            var converters = new List<JsonConverter> {new XenRefListConverter<VM>()};
            var serializer = CreateSerializer(converters);
            return Rpc<List<XenRef<VM>>>("host.get_resident_VMs", new JArray(session, _host ?? ""), serializer);
        }

        public Dictionary<string, string> host_get_logging(string session, string _host)
        {
            var converters = new List<JsonConverter> {new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<string, string>>("host.get_logging", new JArray(session, _host ?? ""), serializer);
        }

        public List<XenRef<PIF>> host_get_pifs(string session, string _host)
        {
            var converters = new List<JsonConverter> {new XenRefListConverter<PIF>()};
            var serializer = CreateSerializer(converters);
            return Rpc<List<XenRef<PIF>>>("host.get_PIFs", new JArray(session, _host ?? ""), serializer);
        }

        public XenRef<SR> host_get_suspend_image_sr(string session, string _host)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<SR>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<SR>>("host.get_suspend_image_sr", new JArray(session, _host ?? ""), serializer);
        }

        public XenRef<SR> host_get_crash_dump_sr(string session, string _host)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<SR>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<SR>>("host.get_crash_dump_sr", new JArray(session, _host ?? ""), serializer);
        }

        public List<XenRef<Host_crashdump>> host_get_crashdumps(string session, string _host)
        {
            var converters = new List<JsonConverter> {new XenRefListConverter<Host_crashdump>()};
            var serializer = CreateSerializer(converters);
            return Rpc<List<XenRef<Host_crashdump>>>("host.get_crashdumps", new JArray(session, _host ?? ""), serializer);
        }

        public List<XenRef<Host_patch>> host_get_patches(string session, string _host)
        {
            var converters = new List<JsonConverter> {new XenRefListConverter<Host_patch>()};
            var serializer = CreateSerializer(converters);
            return Rpc<List<XenRef<Host_patch>>>("host.get_patches", new JArray(session, _host ?? ""), serializer);
        }

        public List<XenRef<Pool_update>> host_get_updates(string session, string _host)
        {
            var converters = new List<JsonConverter> {new XenRefListConverter<Pool_update>()};
            var serializer = CreateSerializer(converters);
            return Rpc<List<XenRef<Pool_update>>>("host.get_updates", new JArray(session, _host ?? ""), serializer);
        }

        public List<XenRef<PBD>> host_get_pbds(string session, string _host)
        {
            var converters = new List<JsonConverter> {new XenRefListConverter<PBD>()};
            var serializer = CreateSerializer(converters);
            return Rpc<List<XenRef<PBD>>>("host.get_PBDs", new JArray(session, _host ?? ""), serializer);
        }

        public List<XenRef<Host_cpu>> host_get_host_cpus(string session, string _host)
        {
            var converters = new List<JsonConverter> {new XenRefListConverter<Host_cpu>()};
            var serializer = CreateSerializer(converters);
            return Rpc<List<XenRef<Host_cpu>>>("host.get_host_CPUs", new JArray(session, _host ?? ""), serializer);
        }

        public Dictionary<string, string> host_get_cpu_info(string session, string _host)
        {
            var converters = new List<JsonConverter> {new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<string, string>>("host.get_cpu_info", new JArray(session, _host ?? ""), serializer);
        }

        public string host_get_hostname(string session, string _host)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("host.get_hostname", new JArray(session, _host ?? ""), serializer);
        }

        public string host_get_address(string session, string _host)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("host.get_address", new JArray(session, _host ?? ""), serializer);
        }

        public XenRef<Host_metrics> host_get_metrics(string session, string _host)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Host_metrics>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Host_metrics>>("host.get_metrics", new JArray(session, _host ?? ""), serializer);
        }

        public Dictionary<string, string> host_get_license_params(string session, string _host)
        {
            var converters = new List<JsonConverter> {new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<string, string>>("host.get_license_params", new JArray(session, _host ?? ""), serializer);
        }

        public string[] host_get_ha_statefiles(string session, string _host)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string[]>("host.get_ha_statefiles", new JArray(session, _host ?? ""), serializer);
        }

        public string[] host_get_ha_network_peers(string session, string _host)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string[]>("host.get_ha_network_peers", new JArray(session, _host ?? ""), serializer);
        }

        public Dictionary<string, XenRef<Blob>> host_get_blobs(string session, string _host)
        {
            var converters = new List<JsonConverter> {new StringXenRefMapConverter<Blob>()};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<string, XenRef<Blob>>>("host.get_blobs", new JArray(session, _host ?? ""), serializer);
        }

        public string[] host_get_tags(string session, string _host)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string[]>("host.get_tags", new JArray(session, _host ?? ""), serializer);
        }

        public string host_get_external_auth_type(string session, string _host)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("host.get_external_auth_type", new JArray(session, _host ?? ""), serializer);
        }

        public string host_get_external_auth_service_name(string session, string _host)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("host.get_external_auth_service_name", new JArray(session, _host ?? ""), serializer);
        }

        public Dictionary<string, string> host_get_external_auth_configuration(string session, string _host)
        {
            var converters = new List<JsonConverter> {new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<string, string>>("host.get_external_auth_configuration", new JArray(session, _host ?? ""), serializer);
        }

        public string host_get_edition(string session, string _host)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("host.get_edition", new JArray(session, _host ?? ""), serializer);
        }

        public Dictionary<string, string> host_get_license_server(string session, string _host)
        {
            var converters = new List<JsonConverter> {new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<string, string>>("host.get_license_server", new JArray(session, _host ?? ""), serializer);
        }

        public Dictionary<string, string> host_get_bios_strings(string session, string _host)
        {
            var converters = new List<JsonConverter> {new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<string, string>>("host.get_bios_strings", new JArray(session, _host ?? ""), serializer);
        }

        public string host_get_power_on_mode(string session, string _host)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("host.get_power_on_mode", new JArray(session, _host ?? ""), serializer);
        }

        public Dictionary<string, string> host_get_power_on_config(string session, string _host)
        {
            var converters = new List<JsonConverter> {new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<string, string>>("host.get_power_on_config", new JArray(session, _host ?? ""), serializer);
        }

        public XenRef<SR> host_get_local_cache_sr(string session, string _host)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<SR>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<SR>>("host.get_local_cache_sr", new JArray(session, _host ?? ""), serializer);
        }

        public Dictionary<string, string> host_get_chipset_info(string session, string _host)
        {
            var converters = new List<JsonConverter> {new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<string, string>>("host.get_chipset_info", new JArray(session, _host ?? ""), serializer);
        }

        public List<XenRef<PCI>> host_get_pcis(string session, string _host)
        {
            var converters = new List<JsonConverter> {new XenRefListConverter<PCI>()};
            var serializer = CreateSerializer(converters);
            return Rpc<List<XenRef<PCI>>>("host.get_PCIs", new JArray(session, _host ?? ""), serializer);
        }

        public List<XenRef<PGPU>> host_get_pgpus(string session, string _host)
        {
            var converters = new List<JsonConverter> {new XenRefListConverter<PGPU>()};
            var serializer = CreateSerializer(converters);
            return Rpc<List<XenRef<PGPU>>>("host.get_PGPUs", new JArray(session, _host ?? ""), serializer);
        }

        public List<XenRef<PUSB>> host_get_pusbs(string session, string _host)
        {
            var converters = new List<JsonConverter> {new XenRefListConverter<PUSB>()};
            var serializer = CreateSerializer(converters);
            return Rpc<List<XenRef<PUSB>>>("host.get_PUSBs", new JArray(session, _host ?? ""), serializer);
        }

        public bool host_get_ssl_legacy(string session, string _host)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<bool>("host.get_ssl_legacy", new JArray(session, _host ?? ""), serializer);
        }

        public Dictionary<string, string> host_get_guest_vcpus_params(string session, string _host)
        {
            var converters = new List<JsonConverter> {new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<string, string>>("host.get_guest_VCPUs_params", new JArray(session, _host ?? ""), serializer);
        }

        public host_display host_get_display(string session, string _host)
        {
            var converters = new List<JsonConverter> {new host_displayConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<host_display>("host.get_display", new JArray(session, _host ?? ""), serializer);
        }

        public long[] host_get_virtual_hardware_platform_versions(string session, string _host)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<long[]>("host.get_virtual_hardware_platform_versions", new JArray(session, _host ?? ""), serializer);
        }

        public XenRef<VM> host_get_control_domain(string session, string _host)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<VM>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<VM>>("host.get_control_domain", new JArray(session, _host ?? ""), serializer);
        }

        public List<XenRef<Pool_update>> host_get_updates_requiring_reboot(string session, string _host)
        {
            var converters = new List<JsonConverter> {new XenRefListConverter<Pool_update>()};
            var serializer = CreateSerializer(converters);
            return Rpc<List<XenRef<Pool_update>>>("host.get_updates_requiring_reboot", new JArray(session, _host ?? ""), serializer);
        }

        public List<XenRef<Feature>> host_get_features(string session, string _host)
        {
            var converters = new List<JsonConverter> {new XenRefListConverter<Feature>()};
            var serializer = CreateSerializer(converters);
            return Rpc<List<XenRef<Feature>>>("host.get_features", new JArray(session, _host ?? ""), serializer);
        }

        public string host_get_iscsi_iqn(string session, string _host)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("host.get_iscsi_iqn", new JArray(session, _host ?? ""), serializer);
        }

        public bool host_get_multipathing(string session, string _host)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<bool>("host.get_multipathing", new JArray(session, _host ?? ""), serializer);
        }

        public void host_set_name_label(string session, string _host, string _label)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("host.set_name_label", new JArray(session, _host ?? "", _label ?? ""), serializer);
        }

        public void host_set_name_description(string session, string _host, string _description)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("host.set_name_description", new JArray(session, _host ?? "", _description ?? ""), serializer);
        }

        public void host_set_other_config(string session, string _host, Dictionary<string, string> _other_config)
        {
            var converters = new List<JsonConverter> {new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            Rpc("host.set_other_config", new JArray(session, _host ?? "", _other_config == null ? new JObject() : JObject.FromObject(_other_config, serializer)), serializer);
        }

        public void host_add_to_other_config(string session, string _host, string _key, string _value)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("host.add_to_other_config", new JArray(session, _host ?? "", _key ?? "", _value ?? ""), serializer);
        }

        public void host_remove_from_other_config(string session, string _host, string _key)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("host.remove_from_other_config", new JArray(session, _host ?? "", _key ?? ""), serializer);
        }

        public void host_set_logging(string session, string _host, Dictionary<string, string> _logging)
        {
            var converters = new List<JsonConverter> {new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            Rpc("host.set_logging", new JArray(session, _host ?? "", _logging == null ? new JObject() : JObject.FromObject(_logging, serializer)), serializer);
        }

        public void host_add_to_logging(string session, string _host, string _key, string _value)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("host.add_to_logging", new JArray(session, _host ?? "", _key ?? "", _value ?? ""), serializer);
        }

        public void host_remove_from_logging(string session, string _host, string _key)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("host.remove_from_logging", new JArray(session, _host ?? "", _key ?? ""), serializer);
        }

        public void host_set_suspend_image_sr(string session, string _host, string _suspend_image_sr)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<SR>()};
            var serializer = CreateSerializer(converters);
            Rpc("host.set_suspend_image_sr", new JArray(session, _host ?? "", _suspend_image_sr ?? ""), serializer);
        }

        public void host_set_crash_dump_sr(string session, string _host, string _crash_dump_sr)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<SR>()};
            var serializer = CreateSerializer(converters);
            Rpc("host.set_crash_dump_sr", new JArray(session, _host ?? "", _crash_dump_sr ?? ""), serializer);
        }

        public void host_set_hostname(string session, string _host, string _hostname)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("host.set_hostname", new JArray(session, _host ?? "", _hostname ?? ""), serializer);
        }

        public void host_set_address(string session, string _host, string _address)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("host.set_address", new JArray(session, _host ?? "", _address ?? ""), serializer);
        }

        public void host_set_tags(string session, string _host, string[] _tags)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("host.set_tags", new JArray(session, _host ?? "", _tags == null ? new JArray() : JArray.FromObject(_tags)), serializer);
        }

        public void host_add_tags(string session, string _host, string _value)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("host.add_tags", new JArray(session, _host ?? "", _value ?? ""), serializer);
        }

        public void host_remove_tags(string session, string _host, string _value)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("host.remove_tags", new JArray(session, _host ?? "", _value ?? ""), serializer);
        }

        public void host_set_license_server(string session, string _host, Dictionary<string, string> _license_server)
        {
            var converters = new List<JsonConverter> {new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            Rpc("host.set_license_server", new JArray(session, _host ?? "", _license_server == null ? new JObject() : JObject.FromObject(_license_server, serializer)), serializer);
        }

        public void host_add_to_license_server(string session, string _host, string _key, string _value)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("host.add_to_license_server", new JArray(session, _host ?? "", _key ?? "", _value ?? ""), serializer);
        }

        public void host_remove_from_license_server(string session, string _host, string _key)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("host.remove_from_license_server", new JArray(session, _host ?? "", _key ?? ""), serializer);
        }

        public void host_set_guest_vcpus_params(string session, string _host, Dictionary<string, string> _guest_vcpus_params)
        {
            var converters = new List<JsonConverter> {new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            Rpc("host.set_guest_VCPUs_params", new JArray(session, _host ?? "", _guest_vcpus_params == null ? new JObject() : JObject.FromObject(_guest_vcpus_params, serializer)), serializer);
        }

        public void host_add_to_guest_vcpus_params(string session, string _host, string _key, string _value)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("host.add_to_guest_VCPUs_params", new JArray(session, _host ?? "", _key ?? "", _value ?? ""), serializer);
        }

        public void host_remove_from_guest_vcpus_params(string session, string _host, string _key)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("host.remove_from_guest_VCPUs_params", new JArray(session, _host ?? "", _key ?? ""), serializer);
        }

        public void host_set_display(string session, string _host, host_display _display)
        {
            var converters = new List<JsonConverter> {new host_displayConverter()};
            var serializer = CreateSerializer(converters);
            Rpc("host.set_display", new JArray(session, _host ?? "", _display.StringOf()), serializer);
        }

        public void host_disable(string session, string _host)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("host.disable", new JArray(session, _host ?? ""), serializer);
        }

        public XenRef<Task> async_host_disable(string session, string _host)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.host.disable", new JArray(session, _host ?? ""), serializer);
        }

        public void host_enable(string session, string _host)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("host.enable", new JArray(session, _host ?? ""), serializer);
        }

        public XenRef<Task> async_host_enable(string session, string _host)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.host.enable", new JArray(session, _host ?? ""), serializer);
        }

        public void host_shutdown(string session, string _host)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("host.shutdown", new JArray(session, _host ?? ""), serializer);
        }

        public XenRef<Task> async_host_shutdown(string session, string _host)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.host.shutdown", new JArray(session, _host ?? ""), serializer);
        }

        public void host_reboot(string session, string _host)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("host.reboot", new JArray(session, _host ?? ""), serializer);
        }

        public XenRef<Task> async_host_reboot(string session, string _host)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.host.reboot", new JArray(session, _host ?? ""), serializer);
        }

        public string host_dmesg(string session, string _host)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("host.dmesg", new JArray(session, _host ?? ""), serializer);
        }

        public XenRef<Task> async_host_dmesg(string session, string _host)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.host.dmesg", new JArray(session, _host ?? ""), serializer);
        }

        public string host_dmesg_clear(string session, string _host)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("host.dmesg_clear", new JArray(session, _host ?? ""), serializer);
        }

        public XenRef<Task> async_host_dmesg_clear(string session, string _host)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.host.dmesg_clear", new JArray(session, _host ?? ""), serializer);
        }

        public string host_get_log(string session, string _host)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("host.get_log", new JArray(session, _host ?? ""), serializer);
        }

        public XenRef<Task> async_host_get_log(string session, string _host)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.host.get_log", new JArray(session, _host ?? ""), serializer);
        }

        public void host_send_debug_keys(string session, string _host, string _keys)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("host.send_debug_keys", new JArray(session, _host ?? "", _keys ?? ""), serializer);
        }

        public XenRef<Task> async_host_send_debug_keys(string session, string _host, string _keys)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.host.send_debug_keys", new JArray(session, _host ?? "", _keys ?? ""), serializer);
        }

        public void host_bugreport_upload(string session, string _host, string _url, Dictionary<string, string> _options)
        {
            var converters = new List<JsonConverter> {new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            Rpc("host.bugreport_upload", new JArray(session, _host ?? "", _url ?? "", _options == null ? new JObject() : JObject.FromObject(_options, serializer)), serializer);
        }

        public XenRef<Task> async_host_bugreport_upload(string session, string _host, string _url, Dictionary<string, string> _options)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>(), new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.host.bugreport_upload", new JArray(session, _host ?? "", _url ?? "", _options == null ? new JObject() : JObject.FromObject(_options, serializer)), serializer);
        }

        public string[] host_list_methods(string session)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string[]>("host.list_methods", new JArray(session), serializer);
        }

        public void host_license_apply(string session, string _host, string _contents)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("host.license_apply", new JArray(session, _host ?? "", _contents ?? ""), serializer);
        }

        public XenRef<Task> async_host_license_apply(string session, string _host, string _contents)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.host.license_apply", new JArray(session, _host ?? "", _contents ?? ""), serializer);
        }

        public void host_license_add(string session, string _host, string _contents)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("host.license_add", new JArray(session, _host ?? "", _contents ?? ""), serializer);
        }

        public XenRef<Task> async_host_license_add(string session, string _host, string _contents)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.host.license_add", new JArray(session, _host ?? "", _contents ?? ""), serializer);
        }

        public void host_license_remove(string session, string _host)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("host.license_remove", new JArray(session, _host ?? ""), serializer);
        }

        public XenRef<Task> async_host_license_remove(string session, string _host)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.host.license_remove", new JArray(session, _host ?? ""), serializer);
        }

        public void host_destroy(string session, string _host)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("host.destroy", new JArray(session, _host ?? ""), serializer);
        }

        public XenRef<Task> async_host_destroy(string session, string _host)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.host.destroy", new JArray(session, _host ?? ""), serializer);
        }

        public void host_power_on(string session, string _host)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("host.power_on", new JArray(session, _host ?? ""), serializer);
        }

        public XenRef<Task> async_host_power_on(string session, string _host)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.host.power_on", new JArray(session, _host ?? ""), serializer);
        }

        public void host_emergency_ha_disable(string session, bool _soft)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("host.emergency_ha_disable", new JArray(session, _soft), serializer);
        }

        public List<Data_source> host_get_data_sources(string session, string _host)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<List<Data_source>>("host.get_data_sources", new JArray(session, _host ?? ""), serializer);
        }

        public void host_record_data_source(string session, string _host, string _data_source)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("host.record_data_source", new JArray(session, _host ?? "", _data_source ?? ""), serializer);
        }

        public double host_query_data_source(string session, string _host, string _data_source)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<double>("host.query_data_source", new JArray(session, _host ?? "", _data_source ?? ""), serializer);
        }

        public void host_forget_data_source_archives(string session, string _host, string _data_source)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("host.forget_data_source_archives", new JArray(session, _host ?? "", _data_source ?? ""), serializer);
        }

        public void host_assert_can_evacuate(string session, string _host)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("host.assert_can_evacuate", new JArray(session, _host ?? ""), serializer);
        }

        public XenRef<Task> async_host_assert_can_evacuate(string session, string _host)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.host.assert_can_evacuate", new JArray(session, _host ?? ""), serializer);
        }

        public Dictionary<XenRef<VM>, string[]> host_get_vms_which_prevent_evacuation(string session, string _host)
        {
            var converters = new List<JsonConverter> {new XenRefStringSetMapConverter<VM>()};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<XenRef<VM>, string[]>>("host.get_vms_which_prevent_evacuation", new JArray(session, _host ?? ""), serializer);
        }

        public XenRef<Task> async_host_get_vms_which_prevent_evacuation(string session, string _host)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.host.get_vms_which_prevent_evacuation", new JArray(session, _host ?? ""), serializer);
        }

        public List<XenRef<VM>> host_get_uncooperative_resident_vms(string session, string _host)
        {
            var converters = new List<JsonConverter> {new XenRefListConverter<VM>()};
            var serializer = CreateSerializer(converters);
            return Rpc<List<XenRef<VM>>>("host.get_uncooperative_resident_VMs", new JArray(session, _host ?? ""), serializer);
        }

        public XenRef<Task> async_host_get_uncooperative_resident_vms(string session, string _host)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.host.get_uncooperative_resident_VMs", new JArray(session, _host ?? ""), serializer);
        }

        public void host_evacuate(string session, string _host)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("host.evacuate", new JArray(session, _host ?? ""), serializer);
        }

        public XenRef<Task> async_host_evacuate(string session, string _host)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.host.evacuate", new JArray(session, _host ?? ""), serializer);
        }

        public void host_syslog_reconfigure(string session, string _host)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("host.syslog_reconfigure", new JArray(session, _host ?? ""), serializer);
        }

        public XenRef<Task> async_host_syslog_reconfigure(string session, string _host)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.host.syslog_reconfigure", new JArray(session, _host ?? ""), serializer);
        }

        public void host_management_reconfigure(string session, string _pif)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<PIF>()};
            var serializer = CreateSerializer(converters);
            Rpc("host.management_reconfigure", new JArray(session, _pif ?? ""), serializer);
        }

        public XenRef<Task> async_host_management_reconfigure(string session, string _pif)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>(), new XenRefConverter<PIF>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.host.management_reconfigure", new JArray(session, _pif ?? ""), serializer);
        }

        public void host_local_management_reconfigure(string session, string _interface)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("host.local_management_reconfigure", new JArray(session, _interface ?? ""), serializer);
        }

        public void host_management_disable(string session)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("host.management_disable", new JArray(session), serializer);
        }

        public XenRef<PIF> host_get_management_interface(string session, string _host)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<PIF>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<PIF>>("host.get_management_interface", new JArray(session, _host ?? ""), serializer);
        }

        public XenRef<Task> async_host_get_management_interface(string session, string _host)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.host.get_management_interface", new JArray(session, _host ?? ""), serializer);
        }

        public string host_get_system_status_capabilities(string session, string _host)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("host.get_system_status_capabilities", new JArray(session, _host ?? ""), serializer);
        }

        public void host_restart_agent(string session, string _host)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("host.restart_agent", new JArray(session, _host ?? ""), serializer);
        }

        public XenRef<Task> async_host_restart_agent(string session, string _host)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.host.restart_agent", new JArray(session, _host ?? ""), serializer);
        }

        public void host_shutdown_agent(string session)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("host.shutdown_agent", new JArray(session), serializer);
        }

        public void host_set_hostname_live(string session, string _host, string _hostname)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("host.set_hostname_live", new JArray(session, _host ?? "", _hostname ?? ""), serializer);
        }

        public long host_compute_free_memory(string session, string _host)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<long>("host.compute_free_memory", new JArray(session, _host ?? ""), serializer);
        }

        public XenRef<Task> async_host_compute_free_memory(string session, string _host)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.host.compute_free_memory", new JArray(session, _host ?? ""), serializer);
        }

        public long host_compute_memory_overhead(string session, string _host)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<long>("host.compute_memory_overhead", new JArray(session, _host ?? ""), serializer);
        }

        public XenRef<Task> async_host_compute_memory_overhead(string session, string _host)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.host.compute_memory_overhead", new JArray(session, _host ?? ""), serializer);
        }

        public void host_sync_data(string session, string _host)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("host.sync_data", new JArray(session, _host ?? ""), serializer);
        }

        public void host_backup_rrds(string session, string _host, double _delay)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("host.backup_rrds", new JArray(session, _host ?? "", _delay), serializer);
        }

        public XenRef<Blob> host_create_new_blob(string session, string _host, string _name, string _mime_type)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Blob>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Blob>>("host.create_new_blob", new JArray(session, _host ?? "", _name ?? "", _mime_type ?? ""), serializer);
        }

        public XenRef<Task> async_host_create_new_blob(string session, string _host, string _name, string _mime_type)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.host.create_new_blob", new JArray(session, _host ?? "", _name ?? "", _mime_type ?? ""), serializer);
        }

        public XenRef<Blob> host_create_new_blob(string session, string _host, string _name, string _mime_type, bool _public)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Blob>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Blob>>("host.create_new_blob", new JArray(session, _host ?? "", _name ?? "", _mime_type ?? "", _public), serializer);
        }

        public XenRef<Task> async_host_create_new_blob(string session, string _host, string _name, string _mime_type, bool _public)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.host.create_new_blob", new JArray(session, _host ?? "", _name ?? "", _mime_type ?? "", _public), serializer);
        }

        public string host_call_plugin(string session, string _host, string _plugin, string _fn, Dictionary<string, string> _args)
        {
            var converters = new List<JsonConverter> {new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("host.call_plugin", new JArray(session, _host ?? "", _plugin ?? "", _fn ?? "", _args == null ? new JObject() : JObject.FromObject(_args, serializer)), serializer);
        }

        public XenRef<Task> async_host_call_plugin(string session, string _host, string _plugin, string _fn, Dictionary<string, string> _args)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>(), new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.host.call_plugin", new JArray(session, _host ?? "", _plugin ?? "", _fn ?? "", _args == null ? new JObject() : JObject.FromObject(_args, serializer)), serializer);
        }

        public bool host_has_extension(string session, string _host, string _name)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<bool>("host.has_extension", new JArray(session, _host ?? "", _name ?? ""), serializer);
        }

        public XenRef<Task> async_host_has_extension(string session, string _host, string _name)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.host.has_extension", new JArray(session, _host ?? "", _name ?? ""), serializer);
        }

        public string host_call_extension(string session, string _host, string _call)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("host.call_extension", new JArray(session, _host ?? "", _call ?? ""), serializer);
        }

        public DateTime host_get_servertime(string session, string _host)
        {
            var converters = new List<JsonConverter> {new XenDateTimeConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<DateTime>("host.get_servertime", new JArray(session, _host ?? ""), serializer);
        }

        public DateTime host_get_server_localtime(string session, string _host)
        {
            var converters = new List<JsonConverter> {new XenDateTimeConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<DateTime>("host.get_server_localtime", new JArray(session, _host ?? ""), serializer);
        }

        public void host_enable_external_auth(string session, string _host, Dictionary<string, string> _config, string _service_name, string _auth_type)
        {
            var converters = new List<JsonConverter> {new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            Rpc("host.enable_external_auth", new JArray(session, _host ?? "", _config == null ? new JObject() : JObject.FromObject(_config, serializer), _service_name ?? "", _auth_type ?? ""), serializer);
        }

        public void host_disable_external_auth(string session, string _host, Dictionary<string, string> _config)
        {
            var converters = new List<JsonConverter> {new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            Rpc("host.disable_external_auth", new JArray(session, _host ?? "", _config == null ? new JObject() : JObject.FromObject(_config, serializer)), serializer);
        }

        public Dictionary<XenRef<VM>, string[]> host_retrieve_wlb_evacuate_recommendations(string session, string _host)
        {
            var converters = new List<JsonConverter> {new XenRefStringSetMapConverter<VM>()};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<XenRef<VM>, string[]>>("host.retrieve_wlb_evacuate_recommendations", new JArray(session, _host ?? ""), serializer);
        }

        public XenRef<Task> async_host_retrieve_wlb_evacuate_recommendations(string session, string _host)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.host.retrieve_wlb_evacuate_recommendations", new JArray(session, _host ?? ""), serializer);
        }

        public string host_get_server_certificate(string session, string _host)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("host.get_server_certificate", new JArray(session, _host ?? ""), serializer);
        }

        public XenRef<Task> async_host_get_server_certificate(string session, string _host)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.host.get_server_certificate", new JArray(session, _host ?? ""), serializer);
        }

        public void host_apply_edition(string session, string _host, string _edition)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("host.apply_edition", new JArray(session, _host ?? "", _edition ?? ""), serializer);
        }

        public void host_apply_edition(string session, string _host, string _edition, bool _force)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("host.apply_edition", new JArray(session, _host ?? "", _edition ?? "", _force), serializer);
        }

        public void host_refresh_pack_info(string session, string _host)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("host.refresh_pack_info", new JArray(session, _host ?? ""), serializer);
        }

        public XenRef<Task> async_host_refresh_pack_info(string session, string _host)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.host.refresh_pack_info", new JArray(session, _host ?? ""), serializer);
        }

        public void host_set_power_on_mode(string session, string _host, string _power_on_mode, Dictionary<string, string> _power_on_config)
        {
            var converters = new List<JsonConverter> {new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            Rpc("host.set_power_on_mode", new JArray(session, _host ?? "", _power_on_mode ?? "", _power_on_config == null ? new JObject() : JObject.FromObject(_power_on_config, serializer)), serializer);
        }

        public XenRef<Task> async_host_set_power_on_mode(string session, string _host, string _power_on_mode, Dictionary<string, string> _power_on_config)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>(), new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.host.set_power_on_mode", new JArray(session, _host ?? "", _power_on_mode ?? "", _power_on_config == null ? new JObject() : JObject.FromObject(_power_on_config, serializer)), serializer);
        }

        public void host_set_cpu_features(string session, string _host, string _features)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("host.set_cpu_features", new JArray(session, _host ?? "", _features ?? ""), serializer);
        }

        public void host_reset_cpu_features(string session, string _host)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("host.reset_cpu_features", new JArray(session, _host ?? ""), serializer);
        }

        public void host_enable_local_storage_caching(string session, string _host, string _sr)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<SR>()};
            var serializer = CreateSerializer(converters);
            Rpc("host.enable_local_storage_caching", new JArray(session, _host ?? "", _sr ?? ""), serializer);
        }

        public void host_disable_local_storage_caching(string session, string _host)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("host.disable_local_storage_caching", new JArray(session, _host ?? ""), serializer);
        }

        public Dictionary<string, string> host_migrate_receive(string session, string _host, string _network, Dictionary<string, string> _options)
        {
            var converters = new List<JsonConverter> {new StringStringMapConverter(), new XenRefConverter<Network>(), new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<string, string>>("host.migrate_receive", new JArray(session, _host ?? "", _network ?? "", _options == null ? new JObject() : JObject.FromObject(_options, serializer)), serializer);
        }

        public XenRef<Task> async_host_migrate_receive(string session, string _host, string _network, Dictionary<string, string> _options)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>(), new XenRefConverter<Network>(), new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.host.migrate_receive", new JArray(session, _host ?? "", _network ?? "", _options == null ? new JObject() : JObject.FromObject(_options, serializer)), serializer);
        }

        public void host_declare_dead(string session, string _host)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("host.declare_dead", new JArray(session, _host ?? ""), serializer);
        }

        public XenRef<Task> async_host_declare_dead(string session, string _host)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.host.declare_dead", new JArray(session, _host ?? ""), serializer);
        }

        public host_display host_enable_display(string session, string _host)
        {
            var converters = new List<JsonConverter> {new host_displayConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<host_display>("host.enable_display", new JArray(session, _host ?? ""), serializer);
        }

        public XenRef<Task> async_host_enable_display(string session, string _host)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.host.enable_display", new JArray(session, _host ?? ""), serializer);
        }

        public host_display host_disable_display(string session, string _host)
        {
            var converters = new List<JsonConverter> {new host_displayConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<host_display>("host.disable_display", new JArray(session, _host ?? ""), serializer);
        }

        public XenRef<Task> async_host_disable_display(string session, string _host)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.host.disable_display", new JArray(session, _host ?? ""), serializer);
        }

        public void host_set_ssl_legacy(string session, string _host, bool _value)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("host.set_ssl_legacy", new JArray(session, _host ?? "", _value), serializer);
        }

        public XenRef<Task> async_host_set_ssl_legacy(string session, string _host, bool _value)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.host.set_ssl_legacy", new JArray(session, _host ?? "", _value), serializer);
        }

        public void host_set_iscsi_iqn(string session, string _host, string _value)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("host.set_iscsi_iqn", new JArray(session, _host ?? "", _value ?? ""), serializer);
        }

        public XenRef<Task> async_host_set_iscsi_iqn(string session, string _host, string _value)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.host.set_iscsi_iqn", new JArray(session, _host ?? "", _value ?? ""), serializer);
        }

        public void host_set_multipathing(string session, string _host, bool _value)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("host.set_multipathing", new JArray(session, _host ?? "", _value), serializer);
        }

        public XenRef<Task> async_host_set_multipathing(string session, string _host, bool _value)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.host.set_multipathing", new JArray(session, _host ?? "", _value), serializer);
        }

        public List<XenRef<Host>> host_get_all(string session)
        {
            var converters = new List<JsonConverter> {new XenRefListConverter<Host>()};
            var serializer = CreateSerializer(converters);
            return Rpc<List<XenRef<Host>>>("host.get_all", new JArray(session), serializer);
        }

        public Dictionary<XenRef<Host>, Host> host_get_all_records(string session)
        {
            var converters = new List<JsonConverter> {new XenRefXenObjectMapConverter<Host>()};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<XenRef<Host>, Host>>("host.get_all_records", new JArray(session), serializer);
        }

        public Host_crashdump host_crashdump_get_record(string session, string _host_crashdump)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<Host_crashdump>("host_crashdump.get_record", new JArray(session, _host_crashdump ?? ""), serializer);
        }

        public XenRef<Host_crashdump> host_crashdump_get_by_uuid(string session, string _uuid)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Host_crashdump>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Host_crashdump>>("host_crashdump.get_by_uuid", new JArray(session, _uuid ?? ""), serializer);
        }

        public string host_crashdump_get_uuid(string session, string _host_crashdump)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("host_crashdump.get_uuid", new JArray(session, _host_crashdump ?? ""), serializer);
        }

        public XenRef<Host> host_crashdump_get_host(string session, string _host_crashdump)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Host>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Host>>("host_crashdump.get_host", new JArray(session, _host_crashdump ?? ""), serializer);
        }

        public DateTime host_crashdump_get_timestamp(string session, string _host_crashdump)
        {
            var converters = new List<JsonConverter> {new XenDateTimeConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<DateTime>("host_crashdump.get_timestamp", new JArray(session, _host_crashdump ?? ""), serializer);
        }

        public long host_crashdump_get_size(string session, string _host_crashdump)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<long>("host_crashdump.get_size", new JArray(session, _host_crashdump ?? ""), serializer);
        }

        public Dictionary<string, string> host_crashdump_get_other_config(string session, string _host_crashdump)
        {
            var converters = new List<JsonConverter> {new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<string, string>>("host_crashdump.get_other_config", new JArray(session, _host_crashdump ?? ""), serializer);
        }

        public void host_crashdump_set_other_config(string session, string _host_crashdump, Dictionary<string, string> _other_config)
        {
            var converters = new List<JsonConverter> {new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            Rpc("host_crashdump.set_other_config", new JArray(session, _host_crashdump ?? "", _other_config == null ? new JObject() : JObject.FromObject(_other_config, serializer)), serializer);
        }

        public void host_crashdump_add_to_other_config(string session, string _host_crashdump, string _key, string _value)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("host_crashdump.add_to_other_config", new JArray(session, _host_crashdump ?? "", _key ?? "", _value ?? ""), serializer);
        }

        public void host_crashdump_remove_from_other_config(string session, string _host_crashdump, string _key)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("host_crashdump.remove_from_other_config", new JArray(session, _host_crashdump ?? "", _key ?? ""), serializer);
        }

        public void host_crashdump_destroy(string session, string _host_crashdump)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("host_crashdump.destroy", new JArray(session, _host_crashdump ?? ""), serializer);
        }

        public XenRef<Task> async_host_crashdump_destroy(string session, string _host_crashdump)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.host_crashdump.destroy", new JArray(session, _host_crashdump ?? ""), serializer);
        }

        public void host_crashdump_upload(string session, string _host_crashdump, string _url, Dictionary<string, string> _options)
        {
            var converters = new List<JsonConverter> {new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            Rpc("host_crashdump.upload", new JArray(session, _host_crashdump ?? "", _url ?? "", _options == null ? new JObject() : JObject.FromObject(_options, serializer)), serializer);
        }

        public XenRef<Task> async_host_crashdump_upload(string session, string _host_crashdump, string _url, Dictionary<string, string> _options)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>(), new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.host_crashdump.upload", new JArray(session, _host_crashdump ?? "", _url ?? "", _options == null ? new JObject() : JObject.FromObject(_options, serializer)), serializer);
        }

        public List<XenRef<Host_crashdump>> host_crashdump_get_all(string session)
        {
            var converters = new List<JsonConverter> {new XenRefListConverter<Host_crashdump>()};
            var serializer = CreateSerializer(converters);
            return Rpc<List<XenRef<Host_crashdump>>>("host_crashdump.get_all", new JArray(session), serializer);
        }

        public Dictionary<XenRef<Host_crashdump>, Host_crashdump> host_crashdump_get_all_records(string session)
        {
            var converters = new List<JsonConverter> {new XenRefXenObjectMapConverter<Host_crashdump>()};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<XenRef<Host_crashdump>, Host_crashdump>>("host_crashdump.get_all_records", new JArray(session), serializer);
        }

        public Host_patch host_patch_get_record(string session, string _host_patch)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<Host_patch>("host_patch.get_record", new JArray(session, _host_patch ?? ""), serializer);
        }

        public XenRef<Host_patch> host_patch_get_by_uuid(string session, string _uuid)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Host_patch>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Host_patch>>("host_patch.get_by_uuid", new JArray(session, _uuid ?? ""), serializer);
        }

        public List<XenRef<Host_patch>> host_patch_get_by_name_label(string session, string _label)
        {
            var converters = new List<JsonConverter> {new XenRefListConverter<Host_patch>()};
            var serializer = CreateSerializer(converters);
            return Rpc<List<XenRef<Host_patch>>>("host_patch.get_by_name_label", new JArray(session, _label ?? ""), serializer);
        }

        public string host_patch_get_uuid(string session, string _host_patch)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("host_patch.get_uuid", new JArray(session, _host_patch ?? ""), serializer);
        }

        public string host_patch_get_name_label(string session, string _host_patch)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("host_patch.get_name_label", new JArray(session, _host_patch ?? ""), serializer);
        }

        public string host_patch_get_name_description(string session, string _host_patch)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("host_patch.get_name_description", new JArray(session, _host_patch ?? ""), serializer);
        }

        public string host_patch_get_version(string session, string _host_patch)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("host_patch.get_version", new JArray(session, _host_patch ?? ""), serializer);
        }

        public XenRef<Host> host_patch_get_host(string session, string _host_patch)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Host>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Host>>("host_patch.get_host", new JArray(session, _host_patch ?? ""), serializer);
        }

        public bool host_patch_get_applied(string session, string _host_patch)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<bool>("host_patch.get_applied", new JArray(session, _host_patch ?? ""), serializer);
        }

        public DateTime host_patch_get_timestamp_applied(string session, string _host_patch)
        {
            var converters = new List<JsonConverter> {new XenDateTimeConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<DateTime>("host_patch.get_timestamp_applied", new JArray(session, _host_patch ?? ""), serializer);
        }

        public long host_patch_get_size(string session, string _host_patch)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<long>("host_patch.get_size", new JArray(session, _host_patch ?? ""), serializer);
        }

        public XenRef<Pool_patch> host_patch_get_pool_patch(string session, string _host_patch)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Pool_patch>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Pool_patch>>("host_patch.get_pool_patch", new JArray(session, _host_patch ?? ""), serializer);
        }

        public Dictionary<string, string> host_patch_get_other_config(string session, string _host_patch)
        {
            var converters = new List<JsonConverter> {new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<string, string>>("host_patch.get_other_config", new JArray(session, _host_patch ?? ""), serializer);
        }

        public void host_patch_set_other_config(string session, string _host_patch, Dictionary<string, string> _other_config)
        {
            var converters = new List<JsonConverter> {new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            Rpc("host_patch.set_other_config", new JArray(session, _host_patch ?? "", _other_config == null ? new JObject() : JObject.FromObject(_other_config, serializer)), serializer);
        }

        public void host_patch_add_to_other_config(string session, string _host_patch, string _key, string _value)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("host_patch.add_to_other_config", new JArray(session, _host_patch ?? "", _key ?? "", _value ?? ""), serializer);
        }

        public void host_patch_remove_from_other_config(string session, string _host_patch, string _key)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("host_patch.remove_from_other_config", new JArray(session, _host_patch ?? "", _key ?? ""), serializer);
        }

        public void host_patch_destroy(string session, string _host_patch)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("host_patch.destroy", new JArray(session, _host_patch ?? ""), serializer);
        }

        public XenRef<Task> async_host_patch_destroy(string session, string _host_patch)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.host_patch.destroy", new JArray(session, _host_patch ?? ""), serializer);
        }

        public string host_patch_apply(string session, string _host_patch)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("host_patch.apply", new JArray(session, _host_patch ?? ""), serializer);
        }

        public XenRef<Task> async_host_patch_apply(string session, string _host_patch)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.host_patch.apply", new JArray(session, _host_patch ?? ""), serializer);
        }

        public List<XenRef<Host_patch>> host_patch_get_all(string session)
        {
            var converters = new List<JsonConverter> {new XenRefListConverter<Host_patch>()};
            var serializer = CreateSerializer(converters);
            return Rpc<List<XenRef<Host_patch>>>("host_patch.get_all", new JArray(session), serializer);
        }

        public Dictionary<XenRef<Host_patch>, Host_patch> host_patch_get_all_records(string session)
        {
            var converters = new List<JsonConverter> {new XenRefXenObjectMapConverter<Host_patch>()};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<XenRef<Host_patch>, Host_patch>>("host_patch.get_all_records", new JArray(session), serializer);
        }

        public Host_metrics host_metrics_get_record(string session, string _host_metrics)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<Host_metrics>("host_metrics.get_record", new JArray(session, _host_metrics ?? ""), serializer);
        }

        public XenRef<Host_metrics> host_metrics_get_by_uuid(string session, string _uuid)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Host_metrics>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Host_metrics>>("host_metrics.get_by_uuid", new JArray(session, _uuid ?? ""), serializer);
        }

        public string host_metrics_get_uuid(string session, string _host_metrics)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("host_metrics.get_uuid", new JArray(session, _host_metrics ?? ""), serializer);
        }

        public long host_metrics_get_memory_total(string session, string _host_metrics)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<long>("host_metrics.get_memory_total", new JArray(session, _host_metrics ?? ""), serializer);
        }

        public long host_metrics_get_memory_free(string session, string _host_metrics)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<long>("host_metrics.get_memory_free", new JArray(session, _host_metrics ?? ""), serializer);
        }

        public bool host_metrics_get_live(string session, string _host_metrics)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<bool>("host_metrics.get_live", new JArray(session, _host_metrics ?? ""), serializer);
        }

        public DateTime host_metrics_get_last_updated(string session, string _host_metrics)
        {
            var converters = new List<JsonConverter> {new XenDateTimeConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<DateTime>("host_metrics.get_last_updated", new JArray(session, _host_metrics ?? ""), serializer);
        }

        public Dictionary<string, string> host_metrics_get_other_config(string session, string _host_metrics)
        {
            var converters = new List<JsonConverter> {new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<string, string>>("host_metrics.get_other_config", new JArray(session, _host_metrics ?? ""), serializer);
        }

        public void host_metrics_set_other_config(string session, string _host_metrics, Dictionary<string, string> _other_config)
        {
            var converters = new List<JsonConverter> {new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            Rpc("host_metrics.set_other_config", new JArray(session, _host_metrics ?? "", _other_config == null ? new JObject() : JObject.FromObject(_other_config, serializer)), serializer);
        }

        public void host_metrics_add_to_other_config(string session, string _host_metrics, string _key, string _value)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("host_metrics.add_to_other_config", new JArray(session, _host_metrics ?? "", _key ?? "", _value ?? ""), serializer);
        }

        public void host_metrics_remove_from_other_config(string session, string _host_metrics, string _key)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("host_metrics.remove_from_other_config", new JArray(session, _host_metrics ?? "", _key ?? ""), serializer);
        }

        public List<XenRef<Host_metrics>> host_metrics_get_all(string session)
        {
            var converters = new List<JsonConverter> {new XenRefListConverter<Host_metrics>()};
            var serializer = CreateSerializer(converters);
            return Rpc<List<XenRef<Host_metrics>>>("host_metrics.get_all", new JArray(session), serializer);
        }

        public Dictionary<XenRef<Host_metrics>, Host_metrics> host_metrics_get_all_records(string session)
        {
            var converters = new List<JsonConverter> {new XenRefXenObjectMapConverter<Host_metrics>()};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<XenRef<Host_metrics>, Host_metrics>>("host_metrics.get_all_records", new JArray(session), serializer);
        }

        public Host_cpu host_cpu_get_record(string session, string _host_cpu)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<Host_cpu>("host_cpu.get_record", new JArray(session, _host_cpu ?? ""), serializer);
        }

        public XenRef<Host_cpu> host_cpu_get_by_uuid(string session, string _uuid)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Host_cpu>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Host_cpu>>("host_cpu.get_by_uuid", new JArray(session, _uuid ?? ""), serializer);
        }

        public string host_cpu_get_uuid(string session, string _host_cpu)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("host_cpu.get_uuid", new JArray(session, _host_cpu ?? ""), serializer);
        }

        public XenRef<Host> host_cpu_get_host(string session, string _host_cpu)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Host>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Host>>("host_cpu.get_host", new JArray(session, _host_cpu ?? ""), serializer);
        }

        public long host_cpu_get_number(string session, string _host_cpu)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<long>("host_cpu.get_number", new JArray(session, _host_cpu ?? ""), serializer);
        }

        public string host_cpu_get_vendor(string session, string _host_cpu)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("host_cpu.get_vendor", new JArray(session, _host_cpu ?? ""), serializer);
        }

        public long host_cpu_get_speed(string session, string _host_cpu)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<long>("host_cpu.get_speed", new JArray(session, _host_cpu ?? ""), serializer);
        }

        public string host_cpu_get_modelname(string session, string _host_cpu)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("host_cpu.get_modelname", new JArray(session, _host_cpu ?? ""), serializer);
        }

        public long host_cpu_get_family(string session, string _host_cpu)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<long>("host_cpu.get_family", new JArray(session, _host_cpu ?? ""), serializer);
        }

        public long host_cpu_get_model(string session, string _host_cpu)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<long>("host_cpu.get_model", new JArray(session, _host_cpu ?? ""), serializer);
        }

        public string host_cpu_get_stepping(string session, string _host_cpu)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("host_cpu.get_stepping", new JArray(session, _host_cpu ?? ""), serializer);
        }

        public string host_cpu_get_flags(string session, string _host_cpu)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("host_cpu.get_flags", new JArray(session, _host_cpu ?? ""), serializer);
        }

        public string host_cpu_get_features(string session, string _host_cpu)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("host_cpu.get_features", new JArray(session, _host_cpu ?? ""), serializer);
        }

        public double host_cpu_get_utilisation(string session, string _host_cpu)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<double>("host_cpu.get_utilisation", new JArray(session, _host_cpu ?? ""), serializer);
        }

        public Dictionary<string, string> host_cpu_get_other_config(string session, string _host_cpu)
        {
            var converters = new List<JsonConverter> {new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<string, string>>("host_cpu.get_other_config", new JArray(session, _host_cpu ?? ""), serializer);
        }

        public void host_cpu_set_other_config(string session, string _host_cpu, Dictionary<string, string> _other_config)
        {
            var converters = new List<JsonConverter> {new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            Rpc("host_cpu.set_other_config", new JArray(session, _host_cpu ?? "", _other_config == null ? new JObject() : JObject.FromObject(_other_config, serializer)), serializer);
        }

        public void host_cpu_add_to_other_config(string session, string _host_cpu, string _key, string _value)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("host_cpu.add_to_other_config", new JArray(session, _host_cpu ?? "", _key ?? "", _value ?? ""), serializer);
        }

        public void host_cpu_remove_from_other_config(string session, string _host_cpu, string _key)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("host_cpu.remove_from_other_config", new JArray(session, _host_cpu ?? "", _key ?? ""), serializer);
        }

        public List<XenRef<Host_cpu>> host_cpu_get_all(string session)
        {
            var converters = new List<JsonConverter> {new XenRefListConverter<Host_cpu>()};
            var serializer = CreateSerializer(converters);
            return Rpc<List<XenRef<Host_cpu>>>("host_cpu.get_all", new JArray(session), serializer);
        }

        public Dictionary<XenRef<Host_cpu>, Host_cpu> host_cpu_get_all_records(string session)
        {
            var converters = new List<JsonConverter> {new XenRefXenObjectMapConverter<Host_cpu>()};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<XenRef<Host_cpu>, Host_cpu>>("host_cpu.get_all_records", new JArray(session), serializer);
        }

        public Network network_get_record(string session, string _network)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<Network>("network.get_record", new JArray(session, _network ?? ""), serializer);
        }

        public XenRef<Network> network_get_by_uuid(string session, string _uuid)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Network>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Network>>("network.get_by_uuid", new JArray(session, _uuid ?? ""), serializer);
        }

        public XenRef<Network> network_create(string session, Network _record)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Network>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Network>>("network.create", new JArray(session, _record.ToJObject()), serializer);
        }

        public XenRef<Task> async_network_create(string session, Network _record)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.network.create", new JArray(session, _record.ToJObject()), serializer);
        }

        public void network_destroy(string session, string _network)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("network.destroy", new JArray(session, _network ?? ""), serializer);
        }

        public XenRef<Task> async_network_destroy(string session, string _network)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.network.destroy", new JArray(session, _network ?? ""), serializer);
        }

        public List<XenRef<Network>> network_get_by_name_label(string session, string _label)
        {
            var converters = new List<JsonConverter> {new XenRefListConverter<Network>()};
            var serializer = CreateSerializer(converters);
            return Rpc<List<XenRef<Network>>>("network.get_by_name_label", new JArray(session, _label ?? ""), serializer);
        }

        public string network_get_uuid(string session, string _network)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("network.get_uuid", new JArray(session, _network ?? ""), serializer);
        }

        public string network_get_name_label(string session, string _network)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("network.get_name_label", new JArray(session, _network ?? ""), serializer);
        }

        public string network_get_name_description(string session, string _network)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("network.get_name_description", new JArray(session, _network ?? ""), serializer);
        }

        public List<network_operations> network_get_allowed_operations(string session, string _network)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<List<network_operations>>("network.get_allowed_operations", new JArray(session, _network ?? ""), serializer);
        }

        public Dictionary<string, network_operations> network_get_current_operations(string session, string _network)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<string, network_operations>>("network.get_current_operations", new JArray(session, _network ?? ""), serializer);
        }

        public List<XenRef<VIF>> network_get_vifs(string session, string _network)
        {
            var converters = new List<JsonConverter> {new XenRefListConverter<VIF>()};
            var serializer = CreateSerializer(converters);
            return Rpc<List<XenRef<VIF>>>("network.get_VIFs", new JArray(session, _network ?? ""), serializer);
        }

        public List<XenRef<PIF>> network_get_pifs(string session, string _network)
        {
            var converters = new List<JsonConverter> {new XenRefListConverter<PIF>()};
            var serializer = CreateSerializer(converters);
            return Rpc<List<XenRef<PIF>>>("network.get_PIFs", new JArray(session, _network ?? ""), serializer);
        }

        public long network_get_mtu(string session, string _network)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<long>("network.get_MTU", new JArray(session, _network ?? ""), serializer);
        }

        public Dictionary<string, string> network_get_other_config(string session, string _network)
        {
            var converters = new List<JsonConverter> {new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<string, string>>("network.get_other_config", new JArray(session, _network ?? ""), serializer);
        }

        public string network_get_bridge(string session, string _network)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("network.get_bridge", new JArray(session, _network ?? ""), serializer);
        }

        public bool network_get_managed(string session, string _network)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<bool>("network.get_managed", new JArray(session, _network ?? ""), serializer);
        }

        public Dictionary<string, XenRef<Blob>> network_get_blobs(string session, string _network)
        {
            var converters = new List<JsonConverter> {new StringXenRefMapConverter<Blob>()};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<string, XenRef<Blob>>>("network.get_blobs", new JArray(session, _network ?? ""), serializer);
        }

        public string[] network_get_tags(string session, string _network)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string[]>("network.get_tags", new JArray(session, _network ?? ""), serializer);
        }

        public network_default_locking_mode network_get_default_locking_mode(string session, string _network)
        {
            var converters = new List<JsonConverter> {new network_default_locking_modeConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<network_default_locking_mode>("network.get_default_locking_mode", new JArray(session, _network ?? ""), serializer);
        }

        public Dictionary<XenRef<VIF>, string> network_get_assigned_ips(string session, string _network)
        {
            var converters = new List<JsonConverter> {new XenRefStringMapConverter<VIF>()};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<XenRef<VIF>, string>>("network.get_assigned_ips", new JArray(session, _network ?? ""), serializer);
        }

        public List<network_purpose> network_get_purpose(string session, string _network)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<List<network_purpose>>("network.get_purpose", new JArray(session, _network ?? ""), serializer);
        }

        public void network_set_name_label(string session, string _network, string _label)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("network.set_name_label", new JArray(session, _network ?? "", _label ?? ""), serializer);
        }

        public void network_set_name_description(string session, string _network, string _description)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("network.set_name_description", new JArray(session, _network ?? "", _description ?? ""), serializer);
        }

        public void network_set_mtu(string session, string _network, long _mtu)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("network.set_MTU", new JArray(session, _network ?? "", _mtu), serializer);
        }

        public void network_set_other_config(string session, string _network, Dictionary<string, string> _other_config)
        {
            var converters = new List<JsonConverter> {new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            Rpc("network.set_other_config", new JArray(session, _network ?? "", _other_config == null ? new JObject() : JObject.FromObject(_other_config, serializer)), serializer);
        }

        public void network_add_to_other_config(string session, string _network, string _key, string _value)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("network.add_to_other_config", new JArray(session, _network ?? "", _key ?? "", _value ?? ""), serializer);
        }

        public void network_remove_from_other_config(string session, string _network, string _key)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("network.remove_from_other_config", new JArray(session, _network ?? "", _key ?? ""), serializer);
        }

        public void network_set_tags(string session, string _network, string[] _tags)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("network.set_tags", new JArray(session, _network ?? "", _tags == null ? new JArray() : JArray.FromObject(_tags)), serializer);
        }

        public void network_add_tags(string session, string _network, string _value)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("network.add_tags", new JArray(session, _network ?? "", _value ?? ""), serializer);
        }

        public void network_remove_tags(string session, string _network, string _value)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("network.remove_tags", new JArray(session, _network ?? "", _value ?? ""), serializer);
        }

        public XenRef<Blob> network_create_new_blob(string session, string _network, string _name, string _mime_type)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Blob>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Blob>>("network.create_new_blob", new JArray(session, _network ?? "", _name ?? "", _mime_type ?? ""), serializer);
        }

        public XenRef<Task> async_network_create_new_blob(string session, string _network, string _name, string _mime_type)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.network.create_new_blob", new JArray(session, _network ?? "", _name ?? "", _mime_type ?? ""), serializer);
        }

        public XenRef<Blob> network_create_new_blob(string session, string _network, string _name, string _mime_type, bool _public)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Blob>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Blob>>("network.create_new_blob", new JArray(session, _network ?? "", _name ?? "", _mime_type ?? "", _public), serializer);
        }

        public XenRef<Task> async_network_create_new_blob(string session, string _network, string _name, string _mime_type, bool _public)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.network.create_new_blob", new JArray(session, _network ?? "", _name ?? "", _mime_type ?? "", _public), serializer);
        }

        public void network_set_default_locking_mode(string session, string _network, network_default_locking_mode _value)
        {
            var converters = new List<JsonConverter> {new network_default_locking_modeConverter()};
            var serializer = CreateSerializer(converters);
            Rpc("network.set_default_locking_mode", new JArray(session, _network ?? "", _value.StringOf()), serializer);
        }

        public XenRef<Task> async_network_set_default_locking_mode(string session, string _network, network_default_locking_mode _value)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>(), new network_default_locking_modeConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.network.set_default_locking_mode", new JArray(session, _network ?? "", _value.StringOf()), serializer);
        }

        public void network_add_purpose(string session, string _network, network_purpose _value)
        {
            var converters = new List<JsonConverter> {new network_purposeConverter()};
            var serializer = CreateSerializer(converters);
            Rpc("network.add_purpose", new JArray(session, _network ?? "", _value.StringOf()), serializer);
        }

        public XenRef<Task> async_network_add_purpose(string session, string _network, network_purpose _value)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>(), new network_purposeConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.network.add_purpose", new JArray(session, _network ?? "", _value.StringOf()), serializer);
        }

        public void network_remove_purpose(string session, string _network, network_purpose _value)
        {
            var converters = new List<JsonConverter> {new network_purposeConverter()};
            var serializer = CreateSerializer(converters);
            Rpc("network.remove_purpose", new JArray(session, _network ?? "", _value.StringOf()), serializer);
        }

        public XenRef<Task> async_network_remove_purpose(string session, string _network, network_purpose _value)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>(), new network_purposeConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.network.remove_purpose", new JArray(session, _network ?? "", _value.StringOf()), serializer);
        }

        public List<XenRef<Network>> network_get_all(string session)
        {
            var converters = new List<JsonConverter> {new XenRefListConverter<Network>()};
            var serializer = CreateSerializer(converters);
            return Rpc<List<XenRef<Network>>>("network.get_all", new JArray(session), serializer);
        }

        public Dictionary<XenRef<Network>, Network> network_get_all_records(string session)
        {
            var converters = new List<JsonConverter> {new XenRefXenObjectMapConverter<Network>()};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<XenRef<Network>, Network>>("network.get_all_records", new JArray(session), serializer);
        }

        public VIF vif_get_record(string session, string _vif)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<VIF>("VIF.get_record", new JArray(session, _vif ?? ""), serializer);
        }

        public XenRef<VIF> vif_get_by_uuid(string session, string _uuid)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<VIF>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<VIF>>("VIF.get_by_uuid", new JArray(session, _uuid ?? ""), serializer);
        }

        public XenRef<VIF> vif_create(string session, VIF _record)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<VIF>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<VIF>>("VIF.create", new JArray(session, _record.ToJObject()), serializer);
        }

        public XenRef<Task> async_vif_create(string session, VIF _record)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.VIF.create", new JArray(session, _record.ToJObject()), serializer);
        }

        public void vif_destroy(string session, string _vif)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("VIF.destroy", new JArray(session, _vif ?? ""), serializer);
        }

        public XenRef<Task> async_vif_destroy(string session, string _vif)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.VIF.destroy", new JArray(session, _vif ?? ""), serializer);
        }

        public string vif_get_uuid(string session, string _vif)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("VIF.get_uuid", new JArray(session, _vif ?? ""), serializer);
        }

        public List<vif_operations> vif_get_allowed_operations(string session, string _vif)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<List<vif_operations>>("VIF.get_allowed_operations", new JArray(session, _vif ?? ""), serializer);
        }

        public Dictionary<string, vif_operations> vif_get_current_operations(string session, string _vif)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<string, vif_operations>>("VIF.get_current_operations", new JArray(session, _vif ?? ""), serializer);
        }

        public string vif_get_device(string session, string _vif)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("VIF.get_device", new JArray(session, _vif ?? ""), serializer);
        }

        public XenRef<Network> vif_get_network(string session, string _vif)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Network>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Network>>("VIF.get_network", new JArray(session, _vif ?? ""), serializer);
        }

        public XenRef<VM> vif_get_vm(string session, string _vif)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<VM>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<VM>>("VIF.get_VM", new JArray(session, _vif ?? ""), serializer);
        }

        public string vif_get_mac(string session, string _vif)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("VIF.get_MAC", new JArray(session, _vif ?? ""), serializer);
        }

        public long vif_get_mtu(string session, string _vif)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<long>("VIF.get_MTU", new JArray(session, _vif ?? ""), serializer);
        }

        public Dictionary<string, string> vif_get_other_config(string session, string _vif)
        {
            var converters = new List<JsonConverter> {new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<string, string>>("VIF.get_other_config", new JArray(session, _vif ?? ""), serializer);
        }

        public bool vif_get_currently_attached(string session, string _vif)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<bool>("VIF.get_currently_attached", new JArray(session, _vif ?? ""), serializer);
        }

        public long vif_get_status_code(string session, string _vif)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<long>("VIF.get_status_code", new JArray(session, _vif ?? ""), serializer);
        }

        public string vif_get_status_detail(string session, string _vif)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("VIF.get_status_detail", new JArray(session, _vif ?? ""), serializer);
        }

        public Dictionary<string, string> vif_get_runtime_properties(string session, string _vif)
        {
            var converters = new List<JsonConverter> {new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<string, string>>("VIF.get_runtime_properties", new JArray(session, _vif ?? ""), serializer);
        }

        public string vif_get_qos_algorithm_type(string session, string _vif)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("VIF.get_qos_algorithm_type", new JArray(session, _vif ?? ""), serializer);
        }

        public Dictionary<string, string> vif_get_qos_algorithm_params(string session, string _vif)
        {
            var converters = new List<JsonConverter> {new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<string, string>>("VIF.get_qos_algorithm_params", new JArray(session, _vif ?? ""), serializer);
        }

        public string[] vif_get_qos_supported_algorithms(string session, string _vif)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string[]>("VIF.get_qos_supported_algorithms", new JArray(session, _vif ?? ""), serializer);
        }

        public XenRef<VIF_metrics> vif_get_metrics(string session, string _vif)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<VIF_metrics>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<VIF_metrics>>("VIF.get_metrics", new JArray(session, _vif ?? ""), serializer);
        }

        public bool vif_get_mac_autogenerated(string session, string _vif)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<bool>("VIF.get_MAC_autogenerated", new JArray(session, _vif ?? ""), serializer);
        }

        public vif_locking_mode vif_get_locking_mode(string session, string _vif)
        {
            var converters = new List<JsonConverter> {new vif_locking_modeConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<vif_locking_mode>("VIF.get_locking_mode", new JArray(session, _vif ?? ""), serializer);
        }

        public string[] vif_get_ipv4_allowed(string session, string _vif)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string[]>("VIF.get_ipv4_allowed", new JArray(session, _vif ?? ""), serializer);
        }

        public string[] vif_get_ipv6_allowed(string session, string _vif)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string[]>("VIF.get_ipv6_allowed", new JArray(session, _vif ?? ""), serializer);
        }

        public vif_ipv4_configuration_mode vif_get_ipv4_configuration_mode(string session, string _vif)
        {
            var converters = new List<JsonConverter> {new vif_ipv4_configuration_modeConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<vif_ipv4_configuration_mode>("VIF.get_ipv4_configuration_mode", new JArray(session, _vif ?? ""), serializer);
        }

        public string[] vif_get_ipv4_addresses(string session, string _vif)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string[]>("VIF.get_ipv4_addresses", new JArray(session, _vif ?? ""), serializer);
        }

        public string vif_get_ipv4_gateway(string session, string _vif)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("VIF.get_ipv4_gateway", new JArray(session, _vif ?? ""), serializer);
        }

        public vif_ipv6_configuration_mode vif_get_ipv6_configuration_mode(string session, string _vif)
        {
            var converters = new List<JsonConverter> {new vif_ipv6_configuration_modeConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<vif_ipv6_configuration_mode>("VIF.get_ipv6_configuration_mode", new JArray(session, _vif ?? ""), serializer);
        }

        public string[] vif_get_ipv6_addresses(string session, string _vif)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string[]>("VIF.get_ipv6_addresses", new JArray(session, _vif ?? ""), serializer);
        }

        public string vif_get_ipv6_gateway(string session, string _vif)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("VIF.get_ipv6_gateway", new JArray(session, _vif ?? ""), serializer);
        }

        public void vif_set_other_config(string session, string _vif, Dictionary<string, string> _other_config)
        {
            var converters = new List<JsonConverter> {new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            Rpc("VIF.set_other_config", new JArray(session, _vif ?? "", _other_config == null ? new JObject() : JObject.FromObject(_other_config, serializer)), serializer);
        }

        public void vif_add_to_other_config(string session, string _vif, string _key, string _value)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("VIF.add_to_other_config", new JArray(session, _vif ?? "", _key ?? "", _value ?? ""), serializer);
        }

        public void vif_remove_from_other_config(string session, string _vif, string _key)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("VIF.remove_from_other_config", new JArray(session, _vif ?? "", _key ?? ""), serializer);
        }

        public void vif_set_qos_algorithm_type(string session, string _vif, string _algorithm_type)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("VIF.set_qos_algorithm_type", new JArray(session, _vif ?? "", _algorithm_type ?? ""), serializer);
        }

        public void vif_set_qos_algorithm_params(string session, string _vif, Dictionary<string, string> _algorithm_params)
        {
            var converters = new List<JsonConverter> {new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            Rpc("VIF.set_qos_algorithm_params", new JArray(session, _vif ?? "", _algorithm_params == null ? new JObject() : JObject.FromObject(_algorithm_params, serializer)), serializer);
        }

        public void vif_add_to_qos_algorithm_params(string session, string _vif, string _key, string _value)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("VIF.add_to_qos_algorithm_params", new JArray(session, _vif ?? "", _key ?? "", _value ?? ""), serializer);
        }

        public void vif_remove_from_qos_algorithm_params(string session, string _vif, string _key)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("VIF.remove_from_qos_algorithm_params", new JArray(session, _vif ?? "", _key ?? ""), serializer);
        }

        public void vif_plug(string session, string _vif)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("VIF.plug", new JArray(session, _vif ?? ""), serializer);
        }

        public XenRef<Task> async_vif_plug(string session, string _vif)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.VIF.plug", new JArray(session, _vif ?? ""), serializer);
        }

        public void vif_unplug(string session, string _vif)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("VIF.unplug", new JArray(session, _vif ?? ""), serializer);
        }

        public XenRef<Task> async_vif_unplug(string session, string _vif)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.VIF.unplug", new JArray(session, _vif ?? ""), serializer);
        }

        public void vif_unplug_force(string session, string _vif)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("VIF.unplug_force", new JArray(session, _vif ?? ""), serializer);
        }

        public XenRef<Task> async_vif_unplug_force(string session, string _vif)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.VIF.unplug_force", new JArray(session, _vif ?? ""), serializer);
        }

        public void vif_move(string session, string _vif, string _network)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Network>()};
            var serializer = CreateSerializer(converters);
            Rpc("VIF.move", new JArray(session, _vif ?? "", _network ?? ""), serializer);
        }

        public XenRef<Task> async_vif_move(string session, string _vif, string _network)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>(), new XenRefConverter<Network>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.VIF.move", new JArray(session, _vif ?? "", _network ?? ""), serializer);
        }

        public void vif_set_locking_mode(string session, string _vif, vif_locking_mode _value)
        {
            var converters = new List<JsonConverter> {new vif_locking_modeConverter()};
            var serializer = CreateSerializer(converters);
            Rpc("VIF.set_locking_mode", new JArray(session, _vif ?? "", _value.StringOf()), serializer);
        }

        public XenRef<Task> async_vif_set_locking_mode(string session, string _vif, vif_locking_mode _value)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>(), new vif_locking_modeConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.VIF.set_locking_mode", new JArray(session, _vif ?? "", _value.StringOf()), serializer);
        }

        public void vif_set_ipv4_allowed(string session, string _vif, string[] _value)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("VIF.set_ipv4_allowed", new JArray(session, _vif ?? "", _value == null ? new JArray() : JArray.FromObject(_value)), serializer);
        }

        public XenRef<Task> async_vif_set_ipv4_allowed(string session, string _vif, string[] _value)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.VIF.set_ipv4_allowed", new JArray(session, _vif ?? "", _value == null ? new JArray() : JArray.FromObject(_value)), serializer);
        }

        public void vif_add_ipv4_allowed(string session, string _vif, string _value)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("VIF.add_ipv4_allowed", new JArray(session, _vif ?? "", _value ?? ""), serializer);
        }

        public XenRef<Task> async_vif_add_ipv4_allowed(string session, string _vif, string _value)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.VIF.add_ipv4_allowed", new JArray(session, _vif ?? "", _value ?? ""), serializer);
        }

        public void vif_remove_ipv4_allowed(string session, string _vif, string _value)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("VIF.remove_ipv4_allowed", new JArray(session, _vif ?? "", _value ?? ""), serializer);
        }

        public XenRef<Task> async_vif_remove_ipv4_allowed(string session, string _vif, string _value)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.VIF.remove_ipv4_allowed", new JArray(session, _vif ?? "", _value ?? ""), serializer);
        }

        public void vif_set_ipv6_allowed(string session, string _vif, string[] _value)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("VIF.set_ipv6_allowed", new JArray(session, _vif ?? "", _value == null ? new JArray() : JArray.FromObject(_value)), serializer);
        }

        public XenRef<Task> async_vif_set_ipv6_allowed(string session, string _vif, string[] _value)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.VIF.set_ipv6_allowed", new JArray(session, _vif ?? "", _value == null ? new JArray() : JArray.FromObject(_value)), serializer);
        }

        public void vif_add_ipv6_allowed(string session, string _vif, string _value)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("VIF.add_ipv6_allowed", new JArray(session, _vif ?? "", _value ?? ""), serializer);
        }

        public XenRef<Task> async_vif_add_ipv6_allowed(string session, string _vif, string _value)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.VIF.add_ipv6_allowed", new JArray(session, _vif ?? "", _value ?? ""), serializer);
        }

        public void vif_remove_ipv6_allowed(string session, string _vif, string _value)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("VIF.remove_ipv6_allowed", new JArray(session, _vif ?? "", _value ?? ""), serializer);
        }

        public XenRef<Task> async_vif_remove_ipv6_allowed(string session, string _vif, string _value)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.VIF.remove_ipv6_allowed", new JArray(session, _vif ?? "", _value ?? ""), serializer);
        }

        public void vif_configure_ipv4(string session, string _vif, vif_ipv4_configuration_mode _mode, string _address, string _gateway)
        {
            var converters = new List<JsonConverter> {new vif_ipv4_configuration_modeConverter()};
            var serializer = CreateSerializer(converters);
            Rpc("VIF.configure_ipv4", new JArray(session, _vif ?? "", _mode.StringOf(), _address ?? "", _gateway ?? ""), serializer);
        }

        public XenRef<Task> async_vif_configure_ipv4(string session, string _vif, vif_ipv4_configuration_mode _mode, string _address, string _gateway)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>(), new vif_ipv4_configuration_modeConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.VIF.configure_ipv4", new JArray(session, _vif ?? "", _mode.StringOf(), _address ?? "", _gateway ?? ""), serializer);
        }

        public void vif_configure_ipv6(string session, string _vif, vif_ipv6_configuration_mode _mode, string _address, string _gateway)
        {
            var converters = new List<JsonConverter> {new vif_ipv6_configuration_modeConverter()};
            var serializer = CreateSerializer(converters);
            Rpc("VIF.configure_ipv6", new JArray(session, _vif ?? "", _mode.StringOf(), _address ?? "", _gateway ?? ""), serializer);
        }

        public XenRef<Task> async_vif_configure_ipv6(string session, string _vif, vif_ipv6_configuration_mode _mode, string _address, string _gateway)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>(), new vif_ipv6_configuration_modeConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.VIF.configure_ipv6", new JArray(session, _vif ?? "", _mode.StringOf(), _address ?? "", _gateway ?? ""), serializer);
        }

        public List<XenRef<VIF>> vif_get_all(string session)
        {
            var converters = new List<JsonConverter> {new XenRefListConverter<VIF>()};
            var serializer = CreateSerializer(converters);
            return Rpc<List<XenRef<VIF>>>("VIF.get_all", new JArray(session), serializer);
        }

        public Dictionary<XenRef<VIF>, VIF> vif_get_all_records(string session)
        {
            var converters = new List<JsonConverter> {new XenRefXenObjectMapConverter<VIF>()};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<XenRef<VIF>, VIF>>("VIF.get_all_records", new JArray(session), serializer);
        }

        public VIF_metrics vif_metrics_get_record(string session, string _vif_metrics)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<VIF_metrics>("VIF_metrics.get_record", new JArray(session, _vif_metrics ?? ""), serializer);
        }

        public XenRef<VIF_metrics> vif_metrics_get_by_uuid(string session, string _uuid)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<VIF_metrics>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<VIF_metrics>>("VIF_metrics.get_by_uuid", new JArray(session, _uuid ?? ""), serializer);
        }

        public string vif_metrics_get_uuid(string session, string _vif_metrics)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("VIF_metrics.get_uuid", new JArray(session, _vif_metrics ?? ""), serializer);
        }

        public double vif_metrics_get_io_read_kbs(string session, string _vif_metrics)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<double>("VIF_metrics.get_io_read_kbs", new JArray(session, _vif_metrics ?? ""), serializer);
        }

        public double vif_metrics_get_io_write_kbs(string session, string _vif_metrics)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<double>("VIF_metrics.get_io_write_kbs", new JArray(session, _vif_metrics ?? ""), serializer);
        }

        public DateTime vif_metrics_get_last_updated(string session, string _vif_metrics)
        {
            var converters = new List<JsonConverter> {new XenDateTimeConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<DateTime>("VIF_metrics.get_last_updated", new JArray(session, _vif_metrics ?? ""), serializer);
        }

        public Dictionary<string, string> vif_metrics_get_other_config(string session, string _vif_metrics)
        {
            var converters = new List<JsonConverter> {new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<string, string>>("VIF_metrics.get_other_config", new JArray(session, _vif_metrics ?? ""), serializer);
        }

        public void vif_metrics_set_other_config(string session, string _vif_metrics, Dictionary<string, string> _other_config)
        {
            var converters = new List<JsonConverter> {new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            Rpc("VIF_metrics.set_other_config", new JArray(session, _vif_metrics ?? "", _other_config == null ? new JObject() : JObject.FromObject(_other_config, serializer)), serializer);
        }

        public void vif_metrics_add_to_other_config(string session, string _vif_metrics, string _key, string _value)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("VIF_metrics.add_to_other_config", new JArray(session, _vif_metrics ?? "", _key ?? "", _value ?? ""), serializer);
        }

        public void vif_metrics_remove_from_other_config(string session, string _vif_metrics, string _key)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("VIF_metrics.remove_from_other_config", new JArray(session, _vif_metrics ?? "", _key ?? ""), serializer);
        }

        public List<XenRef<VIF_metrics>> vif_metrics_get_all(string session)
        {
            var converters = new List<JsonConverter> {new XenRefListConverter<VIF_metrics>()};
            var serializer = CreateSerializer(converters);
            return Rpc<List<XenRef<VIF_metrics>>>("VIF_metrics.get_all", new JArray(session), serializer);
        }

        public Dictionary<XenRef<VIF_metrics>, VIF_metrics> vif_metrics_get_all_records(string session)
        {
            var converters = new List<JsonConverter> {new XenRefXenObjectMapConverter<VIF_metrics>()};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<XenRef<VIF_metrics>, VIF_metrics>>("VIF_metrics.get_all_records", new JArray(session), serializer);
        }

        public PIF pif_get_record(string session, string _pif)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<PIF>("PIF.get_record", new JArray(session, _pif ?? ""), serializer);
        }

        public XenRef<PIF> pif_get_by_uuid(string session, string _uuid)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<PIF>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<PIF>>("PIF.get_by_uuid", new JArray(session, _uuid ?? ""), serializer);
        }

        public string pif_get_uuid(string session, string _pif)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("PIF.get_uuid", new JArray(session, _pif ?? ""), serializer);
        }

        public string pif_get_device(string session, string _pif)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("PIF.get_device", new JArray(session, _pif ?? ""), serializer);
        }

        public XenRef<Network> pif_get_network(string session, string _pif)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Network>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Network>>("PIF.get_network", new JArray(session, _pif ?? ""), serializer);
        }

        public XenRef<Host> pif_get_host(string session, string _pif)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Host>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Host>>("PIF.get_host", new JArray(session, _pif ?? ""), serializer);
        }

        public string pif_get_mac(string session, string _pif)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("PIF.get_MAC", new JArray(session, _pif ?? ""), serializer);
        }

        public long pif_get_mtu(string session, string _pif)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<long>("PIF.get_MTU", new JArray(session, _pif ?? ""), serializer);
        }

        public long pif_get_vlan(string session, string _pif)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<long>("PIF.get_VLAN", new JArray(session, _pif ?? ""), serializer);
        }

        public XenRef<PIF_metrics> pif_get_metrics(string session, string _pif)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<PIF_metrics>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<PIF_metrics>>("PIF.get_metrics", new JArray(session, _pif ?? ""), serializer);
        }

        public bool pif_get_physical(string session, string _pif)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<bool>("PIF.get_physical", new JArray(session, _pif ?? ""), serializer);
        }

        public bool pif_get_currently_attached(string session, string _pif)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<bool>("PIF.get_currently_attached", new JArray(session, _pif ?? ""), serializer);
        }

        public ip_configuration_mode pif_get_ip_configuration_mode(string session, string _pif)
        {
            var converters = new List<JsonConverter> {new ip_configuration_modeConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<ip_configuration_mode>("PIF.get_ip_configuration_mode", new JArray(session, _pif ?? ""), serializer);
        }

        public string pif_get_ip(string session, string _pif)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("PIF.get_IP", new JArray(session, _pif ?? ""), serializer);
        }

        public string pif_get_netmask(string session, string _pif)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("PIF.get_netmask", new JArray(session, _pif ?? ""), serializer);
        }

        public string pif_get_gateway(string session, string _pif)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("PIF.get_gateway", new JArray(session, _pif ?? ""), serializer);
        }

        public string pif_get_dns(string session, string _pif)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("PIF.get_DNS", new JArray(session, _pif ?? ""), serializer);
        }

        public XenRef<Bond> pif_get_bond_slave_of(string session, string _pif)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Bond>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Bond>>("PIF.get_bond_slave_of", new JArray(session, _pif ?? ""), serializer);
        }

        public List<XenRef<Bond>> pif_get_bond_master_of(string session, string _pif)
        {
            var converters = new List<JsonConverter> {new XenRefListConverter<Bond>()};
            var serializer = CreateSerializer(converters);
            return Rpc<List<XenRef<Bond>>>("PIF.get_bond_master_of", new JArray(session, _pif ?? ""), serializer);
        }

        public XenRef<VLAN> pif_get_vlan_master_of(string session, string _pif)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<VLAN>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<VLAN>>("PIF.get_VLAN_master_of", new JArray(session, _pif ?? ""), serializer);
        }

        public List<XenRef<VLAN>> pif_get_vlan_slave_of(string session, string _pif)
        {
            var converters = new List<JsonConverter> {new XenRefListConverter<VLAN>()};
            var serializer = CreateSerializer(converters);
            return Rpc<List<XenRef<VLAN>>>("PIF.get_VLAN_slave_of", new JArray(session, _pif ?? ""), serializer);
        }

        public bool pif_get_management(string session, string _pif)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<bool>("PIF.get_management", new JArray(session, _pif ?? ""), serializer);
        }

        public Dictionary<string, string> pif_get_other_config(string session, string _pif)
        {
            var converters = new List<JsonConverter> {new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<string, string>>("PIF.get_other_config", new JArray(session, _pif ?? ""), serializer);
        }

        public bool pif_get_disallow_unplug(string session, string _pif)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<bool>("PIF.get_disallow_unplug", new JArray(session, _pif ?? ""), serializer);
        }

        public List<XenRef<Tunnel>> pif_get_tunnel_access_pif_of(string session, string _pif)
        {
            var converters = new List<JsonConverter> {new XenRefListConverter<Tunnel>()};
            var serializer = CreateSerializer(converters);
            return Rpc<List<XenRef<Tunnel>>>("PIF.get_tunnel_access_PIF_of", new JArray(session, _pif ?? ""), serializer);
        }

        public List<XenRef<Tunnel>> pif_get_tunnel_transport_pif_of(string session, string _pif)
        {
            var converters = new List<JsonConverter> {new XenRefListConverter<Tunnel>()};
            var serializer = CreateSerializer(converters);
            return Rpc<List<XenRef<Tunnel>>>("PIF.get_tunnel_transport_PIF_of", new JArray(session, _pif ?? ""), serializer);
        }

        public ipv6_configuration_mode pif_get_ipv6_configuration_mode(string session, string _pif)
        {
            var converters = new List<JsonConverter> {new ipv6_configuration_modeConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<ipv6_configuration_mode>("PIF.get_ipv6_configuration_mode", new JArray(session, _pif ?? ""), serializer);
        }

        public string[] pif_get_ipv6(string session, string _pif)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string[]>("PIF.get_IPv6", new JArray(session, _pif ?? ""), serializer);
        }

        public string pif_get_ipv6_gateway(string session, string _pif)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("PIF.get_ipv6_gateway", new JArray(session, _pif ?? ""), serializer);
        }

        public primary_address_type pif_get_primary_address_type(string session, string _pif)
        {
            var converters = new List<JsonConverter> {new primary_address_typeConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<primary_address_type>("PIF.get_primary_address_type", new JArray(session, _pif ?? ""), serializer);
        }

        public bool pif_get_managed(string session, string _pif)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<bool>("PIF.get_managed", new JArray(session, _pif ?? ""), serializer);
        }

        public Dictionary<string, string> pif_get_properties(string session, string _pif)
        {
            var converters = new List<JsonConverter> {new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<string, string>>("PIF.get_properties", new JArray(session, _pif ?? ""), serializer);
        }

        public string[] pif_get_capabilities(string session, string _pif)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string[]>("PIF.get_capabilities", new JArray(session, _pif ?? ""), serializer);
        }

        public pif_igmp_status pif_get_igmp_snooping_status(string session, string _pif)
        {
            var converters = new List<JsonConverter> {new pif_igmp_statusConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<pif_igmp_status>("PIF.get_igmp_snooping_status", new JArray(session, _pif ?? ""), serializer);
        }

        public List<XenRef<Network_sriov>> pif_get_sriov_physical_pif_of(string session, string _pif)
        {
            var converters = new List<JsonConverter> {new XenRefListConverter<Network_sriov>()};
            var serializer = CreateSerializer(converters);
            return Rpc<List<XenRef<Network_sriov>>>("PIF.get_sriov_physical_PIF_of", new JArray(session, _pif ?? ""), serializer);
        }

        public List<XenRef<Network_sriov>> pif_get_sriov_logical_pif_of(string session, string _pif)
        {
            var converters = new List<JsonConverter> {new XenRefListConverter<Network_sriov>()};
            var serializer = CreateSerializer(converters);
            return Rpc<List<XenRef<Network_sriov>>>("PIF.get_sriov_logical_PIF_of", new JArray(session, _pif ?? ""), serializer);
        }

        public XenRef<PCI> pif_get_pci(string session, string _pif)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<PCI>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<PCI>>("PIF.get_PCI", new JArray(session, _pif ?? ""), serializer);
        }

        public void pif_set_other_config(string session, string _pif, Dictionary<string, string> _other_config)
        {
            var converters = new List<JsonConverter> {new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            Rpc("PIF.set_other_config", new JArray(session, _pif ?? "", _other_config == null ? new JObject() : JObject.FromObject(_other_config, serializer)), serializer);
        }

        public void pif_add_to_other_config(string session, string _pif, string _key, string _value)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("PIF.add_to_other_config", new JArray(session, _pif ?? "", _key ?? "", _value ?? ""), serializer);
        }

        public void pif_remove_from_other_config(string session, string _pif, string _key)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("PIF.remove_from_other_config", new JArray(session, _pif ?? "", _key ?? ""), serializer);
        }

        public XenRef<PIF> pif_create_vlan(string session, string _device, string _network, string _host, long _vlan)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<PIF>(), new XenRefConverter<Network>(), new XenRefConverter<Host>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<PIF>>("PIF.create_VLAN", new JArray(session, _device ?? "", _network ?? "", _host ?? "", _vlan), serializer);
        }

        public XenRef<Task> async_pif_create_vlan(string session, string _device, string _network, string _host, long _vlan)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>(), new XenRefConverter<Network>(), new XenRefConverter<Host>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.PIF.create_VLAN", new JArray(session, _device ?? "", _network ?? "", _host ?? "", _vlan), serializer);
        }

        public void pif_destroy(string session, string _pif)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("PIF.destroy", new JArray(session, _pif ?? ""), serializer);
        }

        public XenRef<Task> async_pif_destroy(string session, string _pif)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.PIF.destroy", new JArray(session, _pif ?? ""), serializer);
        }

        public void pif_reconfigure_ip(string session, string _pif, ip_configuration_mode _mode, string _ip, string _netmask, string _gateway, string _dns)
        {
            var converters = new List<JsonConverter> {new ip_configuration_modeConverter()};
            var serializer = CreateSerializer(converters);
            Rpc("PIF.reconfigure_ip", new JArray(session, _pif ?? "", _mode.StringOf(), _ip ?? "", _netmask ?? "", _gateway ?? "", _dns ?? ""), serializer);
        }

        public XenRef<Task> async_pif_reconfigure_ip(string session, string _pif, ip_configuration_mode _mode, string _ip, string _netmask, string _gateway, string _dns)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>(), new ip_configuration_modeConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.PIF.reconfigure_ip", new JArray(session, _pif ?? "", _mode.StringOf(), _ip ?? "", _netmask ?? "", _gateway ?? "", _dns ?? ""), serializer);
        }

        public void pif_reconfigure_ipv6(string session, string _pif, ipv6_configuration_mode _mode, string _ipv6, string _gateway, string _dns)
        {
            var converters = new List<JsonConverter> {new ipv6_configuration_modeConverter()};
            var serializer = CreateSerializer(converters);
            Rpc("PIF.reconfigure_ipv6", new JArray(session, _pif ?? "", _mode.StringOf(), _ipv6 ?? "", _gateway ?? "", _dns ?? ""), serializer);
        }

        public XenRef<Task> async_pif_reconfigure_ipv6(string session, string _pif, ipv6_configuration_mode _mode, string _ipv6, string _gateway, string _dns)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>(), new ipv6_configuration_modeConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.PIF.reconfigure_ipv6", new JArray(session, _pif ?? "", _mode.StringOf(), _ipv6 ?? "", _gateway ?? "", _dns ?? ""), serializer);
        }

        public void pif_set_primary_address_type(string session, string _pif, primary_address_type _primary_address_type)
        {
            var converters = new List<JsonConverter> {new primary_address_typeConverter()};
            var serializer = CreateSerializer(converters);
            Rpc("PIF.set_primary_address_type", new JArray(session, _pif ?? "", _primary_address_type.StringOf()), serializer);
        }

        public XenRef<Task> async_pif_set_primary_address_type(string session, string _pif, primary_address_type _primary_address_type)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>(), new primary_address_typeConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.PIF.set_primary_address_type", new JArray(session, _pif ?? "", _primary_address_type.StringOf()), serializer);
        }

        public void pif_scan(string session, string _host)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Host>()};
            var serializer = CreateSerializer(converters);
            Rpc("PIF.scan", new JArray(session, _host ?? ""), serializer);
        }

        public XenRef<Task> async_pif_scan(string session, string _host)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>(), new XenRefConverter<Host>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.PIF.scan", new JArray(session, _host ?? ""), serializer);
        }

        public XenRef<PIF> pif_introduce(string session, string _host, string _mac, string _device)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<PIF>(), new XenRefConverter<Host>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<PIF>>("PIF.introduce", new JArray(session, _host ?? "", _mac ?? "", _device ?? ""), serializer);
        }

        public XenRef<Task> async_pif_introduce(string session, string _host, string _mac, string _device)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>(), new XenRefConverter<Host>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.PIF.introduce", new JArray(session, _host ?? "", _mac ?? "", _device ?? ""), serializer);
        }

        public XenRef<PIF> pif_introduce(string session, string _host, string _mac, string _device, bool _managed)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<PIF>(), new XenRefConverter<Host>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<PIF>>("PIF.introduce", new JArray(session, _host ?? "", _mac ?? "", _device ?? "", _managed), serializer);
        }

        public XenRef<Task> async_pif_introduce(string session, string _host, string _mac, string _device, bool _managed)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>(), new XenRefConverter<Host>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.PIF.introduce", new JArray(session, _host ?? "", _mac ?? "", _device ?? "", _managed), serializer);
        }

        public void pif_forget(string session, string _pif)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("PIF.forget", new JArray(session, _pif ?? ""), serializer);
        }

        public XenRef<Task> async_pif_forget(string session, string _pif)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.PIF.forget", new JArray(session, _pif ?? ""), serializer);
        }

        public void pif_unplug(string session, string _pif)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("PIF.unplug", new JArray(session, _pif ?? ""), serializer);
        }

        public XenRef<Task> async_pif_unplug(string session, string _pif)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.PIF.unplug", new JArray(session, _pif ?? ""), serializer);
        }

        public void pif_set_disallow_unplug(string session, string _pif, bool _value)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("PIF.set_disallow_unplug", new JArray(session, _pif ?? "", _value), serializer);
        }

        public XenRef<Task> async_pif_set_disallow_unplug(string session, string _pif, bool _value)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.PIF.set_disallow_unplug", new JArray(session, _pif ?? "", _value), serializer);
        }

        public void pif_plug(string session, string _pif)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("PIF.plug", new JArray(session, _pif ?? ""), serializer);
        }

        public XenRef<Task> async_pif_plug(string session, string _pif)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.PIF.plug", new JArray(session, _pif ?? ""), serializer);
        }

        public XenRef<PIF> pif_db_introduce(string session, string _device, string _network, string _host, string _mac, long _mtu, long _vlan, bool _physical, ip_configuration_mode _ip_configuration_mode, string _ip, string _netmask, string _gateway, string _dns, string _bond_slave_of, string _vlan_master_of, bool _management, Dictionary<string, string> _other_config, bool _disallow_unplug)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<PIF>(), new XenRefConverter<Network>(), new XenRefConverter<Host>(), new ip_configuration_modeConverter(), new XenRefConverter<Bond>(), new XenRefConverter<VLAN>(), new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<PIF>>("PIF.db_introduce", new JArray(session, _device ?? "", _network ?? "", _host ?? "", _mac ?? "", _mtu, _vlan, _physical, _ip_configuration_mode.StringOf(), _ip ?? "", _netmask ?? "", _gateway ?? "", _dns ?? "", _bond_slave_of ?? "", _vlan_master_of ?? "", _management, _other_config == null ? new JObject() : JObject.FromObject(_other_config, serializer), _disallow_unplug), serializer);
        }

        public XenRef<Task> async_pif_db_introduce(string session, string _device, string _network, string _host, string _mac, long _mtu, long _vlan, bool _physical, ip_configuration_mode _ip_configuration_mode, string _ip, string _netmask, string _gateway, string _dns, string _bond_slave_of, string _vlan_master_of, bool _management, Dictionary<string, string> _other_config, bool _disallow_unplug)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>(), new XenRefConverter<Network>(), new XenRefConverter<Host>(), new ip_configuration_modeConverter(), new XenRefConverter<Bond>(), new XenRefConverter<VLAN>(), new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.PIF.db_introduce", new JArray(session, _device ?? "", _network ?? "", _host ?? "", _mac ?? "", _mtu, _vlan, _physical, _ip_configuration_mode.StringOf(), _ip ?? "", _netmask ?? "", _gateway ?? "", _dns ?? "", _bond_slave_of ?? "", _vlan_master_of ?? "", _management, _other_config == null ? new JObject() : JObject.FromObject(_other_config, serializer), _disallow_unplug), serializer);
        }

        public XenRef<PIF> pif_db_introduce(string session, string _device, string _network, string _host, string _mac, long _mtu, long _vlan, bool _physical, ip_configuration_mode _ip_configuration_mode, string _ip, string _netmask, string _gateway, string _dns, string _bond_slave_of, string _vlan_master_of, bool _management, Dictionary<string, string> _other_config, bool _disallow_unplug, ipv6_configuration_mode _ipv6_configuration_mode, string[] _ipv6, string _ipv6_gateway, primary_address_type _primary_address_type)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<PIF>(), new XenRefConverter<Network>(), new XenRefConverter<Host>(), new ip_configuration_modeConverter(), new XenRefConverter<Bond>(), new XenRefConverter<VLAN>(), new StringStringMapConverter(), new ipv6_configuration_modeConverter(), new primary_address_typeConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<PIF>>("PIF.db_introduce", new JArray(session, _device ?? "", _network ?? "", _host ?? "", _mac ?? "", _mtu, _vlan, _physical, _ip_configuration_mode.StringOf(), _ip ?? "", _netmask ?? "", _gateway ?? "", _dns ?? "", _bond_slave_of ?? "", _vlan_master_of ?? "", _management, _other_config == null ? new JObject() : JObject.FromObject(_other_config, serializer), _disallow_unplug, _ipv6_configuration_mode.StringOf(), _ipv6 == null ? new JArray() : JArray.FromObject(_ipv6), _ipv6_gateway ?? "", _primary_address_type.StringOf()), serializer);
        }

        public XenRef<Task> async_pif_db_introduce(string session, string _device, string _network, string _host, string _mac, long _mtu, long _vlan, bool _physical, ip_configuration_mode _ip_configuration_mode, string _ip, string _netmask, string _gateway, string _dns, string _bond_slave_of, string _vlan_master_of, bool _management, Dictionary<string, string> _other_config, bool _disallow_unplug, ipv6_configuration_mode _ipv6_configuration_mode, string[] _ipv6, string _ipv6_gateway, primary_address_type _primary_address_type)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>(), new XenRefConverter<Network>(), new XenRefConverter<Host>(), new ip_configuration_modeConverter(), new XenRefConverter<Bond>(), new XenRefConverter<VLAN>(), new StringStringMapConverter(), new ipv6_configuration_modeConverter(), new primary_address_typeConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.PIF.db_introduce", new JArray(session, _device ?? "", _network ?? "", _host ?? "", _mac ?? "", _mtu, _vlan, _physical, _ip_configuration_mode.StringOf(), _ip ?? "", _netmask ?? "", _gateway ?? "", _dns ?? "", _bond_slave_of ?? "", _vlan_master_of ?? "", _management, _other_config == null ? new JObject() : JObject.FromObject(_other_config, serializer), _disallow_unplug, _ipv6_configuration_mode.StringOf(), _ipv6 == null ? new JArray() : JArray.FromObject(_ipv6), _ipv6_gateway ?? "", _primary_address_type.StringOf()), serializer);
        }

        public XenRef<PIF> pif_db_introduce(string session, string _device, string _network, string _host, string _mac, long _mtu, long _vlan, bool _physical, ip_configuration_mode _ip_configuration_mode, string _ip, string _netmask, string _gateway, string _dns, string _bond_slave_of, string _vlan_master_of, bool _management, Dictionary<string, string> _other_config, bool _disallow_unplug, ipv6_configuration_mode _ipv6_configuration_mode, string[] _ipv6, string _ipv6_gateway, primary_address_type _primary_address_type, bool _managed)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<PIF>(), new XenRefConverter<Network>(), new XenRefConverter<Host>(), new ip_configuration_modeConverter(), new XenRefConverter<Bond>(), new XenRefConverter<VLAN>(), new StringStringMapConverter(), new ipv6_configuration_modeConverter(), new primary_address_typeConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<PIF>>("PIF.db_introduce", new JArray(session, _device ?? "", _network ?? "", _host ?? "", _mac ?? "", _mtu, _vlan, _physical, _ip_configuration_mode.StringOf(), _ip ?? "", _netmask ?? "", _gateway ?? "", _dns ?? "", _bond_slave_of ?? "", _vlan_master_of ?? "", _management, _other_config == null ? new JObject() : JObject.FromObject(_other_config, serializer), _disallow_unplug, _ipv6_configuration_mode.StringOf(), _ipv6 == null ? new JArray() : JArray.FromObject(_ipv6), _ipv6_gateway ?? "", _primary_address_type.StringOf(), _managed), serializer);
        }

        public XenRef<Task> async_pif_db_introduce(string session, string _device, string _network, string _host, string _mac, long _mtu, long _vlan, bool _physical, ip_configuration_mode _ip_configuration_mode, string _ip, string _netmask, string _gateway, string _dns, string _bond_slave_of, string _vlan_master_of, bool _management, Dictionary<string, string> _other_config, bool _disallow_unplug, ipv6_configuration_mode _ipv6_configuration_mode, string[] _ipv6, string _ipv6_gateway, primary_address_type _primary_address_type, bool _managed)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>(), new XenRefConverter<Network>(), new XenRefConverter<Host>(), new ip_configuration_modeConverter(), new XenRefConverter<Bond>(), new XenRefConverter<VLAN>(), new StringStringMapConverter(), new ipv6_configuration_modeConverter(), new primary_address_typeConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.PIF.db_introduce", new JArray(session, _device ?? "", _network ?? "", _host ?? "", _mac ?? "", _mtu, _vlan, _physical, _ip_configuration_mode.StringOf(), _ip ?? "", _netmask ?? "", _gateway ?? "", _dns ?? "", _bond_slave_of ?? "", _vlan_master_of ?? "", _management, _other_config == null ? new JObject() : JObject.FromObject(_other_config, serializer), _disallow_unplug, _ipv6_configuration_mode.StringOf(), _ipv6 == null ? new JArray() : JArray.FromObject(_ipv6), _ipv6_gateway ?? "", _primary_address_type.StringOf(), _managed), serializer);
        }

        public XenRef<PIF> pif_db_introduce(string session, string _device, string _network, string _host, string _mac, long _mtu, long _vlan, bool _physical, ip_configuration_mode _ip_configuration_mode, string _ip, string _netmask, string _gateway, string _dns, string _bond_slave_of, string _vlan_master_of, bool _management, Dictionary<string, string> _other_config, bool _disallow_unplug, ipv6_configuration_mode _ipv6_configuration_mode, string[] _ipv6, string _ipv6_gateway, primary_address_type _primary_address_type, bool _managed, Dictionary<string, string> _properties)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<PIF>(), new XenRefConverter<Network>(), new XenRefConverter<Host>(), new ip_configuration_modeConverter(), new XenRefConverter<Bond>(), new XenRefConverter<VLAN>(), new StringStringMapConverter(), new ipv6_configuration_modeConverter(), new primary_address_typeConverter(), new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<PIF>>("PIF.db_introduce", new JArray(session, _device ?? "", _network ?? "", _host ?? "", _mac ?? "", _mtu, _vlan, _physical, _ip_configuration_mode.StringOf(), _ip ?? "", _netmask ?? "", _gateway ?? "", _dns ?? "", _bond_slave_of ?? "", _vlan_master_of ?? "", _management, _other_config == null ? new JObject() : JObject.FromObject(_other_config, serializer), _disallow_unplug, _ipv6_configuration_mode.StringOf(), _ipv6 == null ? new JArray() : JArray.FromObject(_ipv6), _ipv6_gateway ?? "", _primary_address_type.StringOf(), _managed, _properties == null ? new JObject() : JObject.FromObject(_properties, serializer)), serializer);
        }

        public XenRef<Task> async_pif_db_introduce(string session, string _device, string _network, string _host, string _mac, long _mtu, long _vlan, bool _physical, ip_configuration_mode _ip_configuration_mode, string _ip, string _netmask, string _gateway, string _dns, string _bond_slave_of, string _vlan_master_of, bool _management, Dictionary<string, string> _other_config, bool _disallow_unplug, ipv6_configuration_mode _ipv6_configuration_mode, string[] _ipv6, string _ipv6_gateway, primary_address_type _primary_address_type, bool _managed, Dictionary<string, string> _properties)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>(), new XenRefConverter<Network>(), new XenRefConverter<Host>(), new ip_configuration_modeConverter(), new XenRefConverter<Bond>(), new XenRefConverter<VLAN>(), new StringStringMapConverter(), new ipv6_configuration_modeConverter(), new primary_address_typeConverter(), new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.PIF.db_introduce", new JArray(session, _device ?? "", _network ?? "", _host ?? "", _mac ?? "", _mtu, _vlan, _physical, _ip_configuration_mode.StringOf(), _ip ?? "", _netmask ?? "", _gateway ?? "", _dns ?? "", _bond_slave_of ?? "", _vlan_master_of ?? "", _management, _other_config == null ? new JObject() : JObject.FromObject(_other_config, serializer), _disallow_unplug, _ipv6_configuration_mode.StringOf(), _ipv6 == null ? new JArray() : JArray.FromObject(_ipv6), _ipv6_gateway ?? "", _primary_address_type.StringOf(), _managed, _properties == null ? new JObject() : JObject.FromObject(_properties, serializer)), serializer);
        }

        public void pif_db_forget(string session, string _pif)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("PIF.db_forget", new JArray(session, _pif ?? ""), serializer);
        }

        public XenRef<Task> async_pif_db_forget(string session, string _pif)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.PIF.db_forget", new JArray(session, _pif ?? ""), serializer);
        }

        public void pif_set_property(string session, string _pif, string _name, string _value)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("PIF.set_property", new JArray(session, _pif ?? "", _name ?? "", _value ?? ""), serializer);
        }

        public XenRef<Task> async_pif_set_property(string session, string _pif, string _name, string _value)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.PIF.set_property", new JArray(session, _pif ?? "", _name ?? "", _value ?? ""), serializer);
        }

        public List<XenRef<PIF>> pif_get_all(string session)
        {
            var converters = new List<JsonConverter> {new XenRefListConverter<PIF>()};
            var serializer = CreateSerializer(converters);
            return Rpc<List<XenRef<PIF>>>("PIF.get_all", new JArray(session), serializer);
        }

        public Dictionary<XenRef<PIF>, PIF> pif_get_all_records(string session)
        {
            var converters = new List<JsonConverter> {new XenRefXenObjectMapConverter<PIF>()};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<XenRef<PIF>, PIF>>("PIF.get_all_records", new JArray(session), serializer);
        }

        public PIF_metrics pif_metrics_get_record(string session, string _pif_metrics)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<PIF_metrics>("PIF_metrics.get_record", new JArray(session, _pif_metrics ?? ""), serializer);
        }

        public XenRef<PIF_metrics> pif_metrics_get_by_uuid(string session, string _uuid)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<PIF_metrics>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<PIF_metrics>>("PIF_metrics.get_by_uuid", new JArray(session, _uuid ?? ""), serializer);
        }

        public string pif_metrics_get_uuid(string session, string _pif_metrics)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("PIF_metrics.get_uuid", new JArray(session, _pif_metrics ?? ""), serializer);
        }

        public double pif_metrics_get_io_read_kbs(string session, string _pif_metrics)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<double>("PIF_metrics.get_io_read_kbs", new JArray(session, _pif_metrics ?? ""), serializer);
        }

        public double pif_metrics_get_io_write_kbs(string session, string _pif_metrics)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<double>("PIF_metrics.get_io_write_kbs", new JArray(session, _pif_metrics ?? ""), serializer);
        }

        public bool pif_metrics_get_carrier(string session, string _pif_metrics)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<bool>("PIF_metrics.get_carrier", new JArray(session, _pif_metrics ?? ""), serializer);
        }

        public string pif_metrics_get_vendor_id(string session, string _pif_metrics)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("PIF_metrics.get_vendor_id", new JArray(session, _pif_metrics ?? ""), serializer);
        }

        public string pif_metrics_get_vendor_name(string session, string _pif_metrics)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("PIF_metrics.get_vendor_name", new JArray(session, _pif_metrics ?? ""), serializer);
        }

        public string pif_metrics_get_device_id(string session, string _pif_metrics)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("PIF_metrics.get_device_id", new JArray(session, _pif_metrics ?? ""), serializer);
        }

        public string pif_metrics_get_device_name(string session, string _pif_metrics)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("PIF_metrics.get_device_name", new JArray(session, _pif_metrics ?? ""), serializer);
        }

        public long pif_metrics_get_speed(string session, string _pif_metrics)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<long>("PIF_metrics.get_speed", new JArray(session, _pif_metrics ?? ""), serializer);
        }

        public bool pif_metrics_get_duplex(string session, string _pif_metrics)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<bool>("PIF_metrics.get_duplex", new JArray(session, _pif_metrics ?? ""), serializer);
        }

        public string pif_metrics_get_pci_bus_path(string session, string _pif_metrics)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("PIF_metrics.get_pci_bus_path", new JArray(session, _pif_metrics ?? ""), serializer);
        }

        public DateTime pif_metrics_get_last_updated(string session, string _pif_metrics)
        {
            var converters = new List<JsonConverter> {new XenDateTimeConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<DateTime>("PIF_metrics.get_last_updated", new JArray(session, _pif_metrics ?? ""), serializer);
        }

        public Dictionary<string, string> pif_metrics_get_other_config(string session, string _pif_metrics)
        {
            var converters = new List<JsonConverter> {new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<string, string>>("PIF_metrics.get_other_config", new JArray(session, _pif_metrics ?? ""), serializer);
        }

        public void pif_metrics_set_other_config(string session, string _pif_metrics, Dictionary<string, string> _other_config)
        {
            var converters = new List<JsonConverter> {new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            Rpc("PIF_metrics.set_other_config", new JArray(session, _pif_metrics ?? "", _other_config == null ? new JObject() : JObject.FromObject(_other_config, serializer)), serializer);
        }

        public void pif_metrics_add_to_other_config(string session, string _pif_metrics, string _key, string _value)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("PIF_metrics.add_to_other_config", new JArray(session, _pif_metrics ?? "", _key ?? "", _value ?? ""), serializer);
        }

        public void pif_metrics_remove_from_other_config(string session, string _pif_metrics, string _key)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("PIF_metrics.remove_from_other_config", new JArray(session, _pif_metrics ?? "", _key ?? ""), serializer);
        }

        public List<XenRef<PIF_metrics>> pif_metrics_get_all(string session)
        {
            var converters = new List<JsonConverter> {new XenRefListConverter<PIF_metrics>()};
            var serializer = CreateSerializer(converters);
            return Rpc<List<XenRef<PIF_metrics>>>("PIF_metrics.get_all", new JArray(session), serializer);
        }

        public Dictionary<XenRef<PIF_metrics>, PIF_metrics> pif_metrics_get_all_records(string session)
        {
            var converters = new List<JsonConverter> {new XenRefXenObjectMapConverter<PIF_metrics>()};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<XenRef<PIF_metrics>, PIF_metrics>>("PIF_metrics.get_all_records", new JArray(session), serializer);
        }

        public Bond bond_get_record(string session, string _bond)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<Bond>("Bond.get_record", new JArray(session, _bond ?? ""), serializer);
        }

        public XenRef<Bond> bond_get_by_uuid(string session, string _uuid)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Bond>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Bond>>("Bond.get_by_uuid", new JArray(session, _uuid ?? ""), serializer);
        }

        public string bond_get_uuid(string session, string _bond)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("Bond.get_uuid", new JArray(session, _bond ?? ""), serializer);
        }

        public XenRef<PIF> bond_get_master(string session, string _bond)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<PIF>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<PIF>>("Bond.get_master", new JArray(session, _bond ?? ""), serializer);
        }

        public List<XenRef<PIF>> bond_get_slaves(string session, string _bond)
        {
            var converters = new List<JsonConverter> {new XenRefListConverter<PIF>()};
            var serializer = CreateSerializer(converters);
            return Rpc<List<XenRef<PIF>>>("Bond.get_slaves", new JArray(session, _bond ?? ""), serializer);
        }

        public Dictionary<string, string> bond_get_other_config(string session, string _bond)
        {
            var converters = new List<JsonConverter> {new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<string, string>>("Bond.get_other_config", new JArray(session, _bond ?? ""), serializer);
        }

        public XenRef<PIF> bond_get_primary_slave(string session, string _bond)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<PIF>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<PIF>>("Bond.get_primary_slave", new JArray(session, _bond ?? ""), serializer);
        }

        public bond_mode bond_get_mode(string session, string _bond)
        {
            var converters = new List<JsonConverter> {new bond_modeConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<bond_mode>("Bond.get_mode", new JArray(session, _bond ?? ""), serializer);
        }

        public Dictionary<string, string> bond_get_properties(string session, string _bond)
        {
            var converters = new List<JsonConverter> {new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<string, string>>("Bond.get_properties", new JArray(session, _bond ?? ""), serializer);
        }

        public long bond_get_links_up(string session, string _bond)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<long>("Bond.get_links_up", new JArray(session, _bond ?? ""), serializer);
        }

        public void bond_set_other_config(string session, string _bond, Dictionary<string, string> _other_config)
        {
            var converters = new List<JsonConverter> {new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            Rpc("Bond.set_other_config", new JArray(session, _bond ?? "", _other_config == null ? new JObject() : JObject.FromObject(_other_config, serializer)), serializer);
        }

        public void bond_add_to_other_config(string session, string _bond, string _key, string _value)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("Bond.add_to_other_config", new JArray(session, _bond ?? "", _key ?? "", _value ?? ""), serializer);
        }

        public void bond_remove_from_other_config(string session, string _bond, string _key)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("Bond.remove_from_other_config", new JArray(session, _bond ?? "", _key ?? ""), serializer);
        }

        public XenRef<Bond> bond_create(string session, string _network, List<XenRef<PIF>> _members, string _mac)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Bond>(), new XenRefConverter<Network>(), new XenRefListConverter<PIF>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Bond>>("Bond.create", new JArray(session, _network ?? "", _members == null ? new JArray() : JArray.FromObject(_members, serializer), _mac ?? ""), serializer);
        }

        public XenRef<Task> async_bond_create(string session, string _network, List<XenRef<PIF>> _members, string _mac)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>(), new XenRefConverter<Network>(), new XenRefListConverter<PIF>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.Bond.create", new JArray(session, _network ?? "", _members == null ? new JArray() : JArray.FromObject(_members, serializer), _mac ?? ""), serializer);
        }

        public XenRef<Bond> bond_create(string session, string _network, List<XenRef<PIF>> _members, string _mac, bond_mode _mode)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Bond>(), new XenRefConverter<Network>(), new XenRefListConverter<PIF>(), new bond_modeConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Bond>>("Bond.create", new JArray(session, _network ?? "", _members == null ? new JArray() : JArray.FromObject(_members, serializer), _mac ?? "", _mode.StringOf()), serializer);
        }

        public XenRef<Task> async_bond_create(string session, string _network, List<XenRef<PIF>> _members, string _mac, bond_mode _mode)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>(), new XenRefConverter<Network>(), new XenRefListConverter<PIF>(), new bond_modeConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.Bond.create", new JArray(session, _network ?? "", _members == null ? new JArray() : JArray.FromObject(_members, serializer), _mac ?? "", _mode.StringOf()), serializer);
        }

        public XenRef<Bond> bond_create(string session, string _network, List<XenRef<PIF>> _members, string _mac, bond_mode _mode, Dictionary<string, string> _properties)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Bond>(), new XenRefConverter<Network>(), new XenRefListConverter<PIF>(), new bond_modeConverter(), new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Bond>>("Bond.create", new JArray(session, _network ?? "", _members == null ? new JArray() : JArray.FromObject(_members, serializer), _mac ?? "", _mode.StringOf(), _properties == null ? new JObject() : JObject.FromObject(_properties, serializer)), serializer);
        }

        public XenRef<Task> async_bond_create(string session, string _network, List<XenRef<PIF>> _members, string _mac, bond_mode _mode, Dictionary<string, string> _properties)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>(), new XenRefConverter<Network>(), new XenRefListConverter<PIF>(), new bond_modeConverter(), new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.Bond.create", new JArray(session, _network ?? "", _members == null ? new JArray() : JArray.FromObject(_members, serializer), _mac ?? "", _mode.StringOf(), _properties == null ? new JObject() : JObject.FromObject(_properties, serializer)), serializer);
        }

        public void bond_destroy(string session, string _bond)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("Bond.destroy", new JArray(session, _bond ?? ""), serializer);
        }

        public XenRef<Task> async_bond_destroy(string session, string _bond)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.Bond.destroy", new JArray(session, _bond ?? ""), serializer);
        }

        public void bond_set_mode(string session, string _bond, bond_mode _value)
        {
            var converters = new List<JsonConverter> {new bond_modeConverter()};
            var serializer = CreateSerializer(converters);
            Rpc("Bond.set_mode", new JArray(session, _bond ?? "", _value.StringOf()), serializer);
        }

        public XenRef<Task> async_bond_set_mode(string session, string _bond, bond_mode _value)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>(), new bond_modeConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.Bond.set_mode", new JArray(session, _bond ?? "", _value.StringOf()), serializer);
        }

        public void bond_set_property(string session, string _bond, string _name, string _value)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("Bond.set_property", new JArray(session, _bond ?? "", _name ?? "", _value ?? ""), serializer);
        }

        public XenRef<Task> async_bond_set_property(string session, string _bond, string _name, string _value)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.Bond.set_property", new JArray(session, _bond ?? "", _name ?? "", _value ?? ""), serializer);
        }

        public List<XenRef<Bond>> bond_get_all(string session)
        {
            var converters = new List<JsonConverter> {new XenRefListConverter<Bond>()};
            var serializer = CreateSerializer(converters);
            return Rpc<List<XenRef<Bond>>>("Bond.get_all", new JArray(session), serializer);
        }

        public Dictionary<XenRef<Bond>, Bond> bond_get_all_records(string session)
        {
            var converters = new List<JsonConverter> {new XenRefXenObjectMapConverter<Bond>()};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<XenRef<Bond>, Bond>>("Bond.get_all_records", new JArray(session), serializer);
        }

        public VLAN vlan_get_record(string session, string _vlan)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<VLAN>("VLAN.get_record", new JArray(session, _vlan ?? ""), serializer);
        }

        public XenRef<VLAN> vlan_get_by_uuid(string session, string _uuid)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<VLAN>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<VLAN>>("VLAN.get_by_uuid", new JArray(session, _uuid ?? ""), serializer);
        }

        public string vlan_get_uuid(string session, string _vlan)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("VLAN.get_uuid", new JArray(session, _vlan ?? ""), serializer);
        }

        public XenRef<PIF> vlan_get_tagged_pif(string session, string _vlan)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<PIF>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<PIF>>("VLAN.get_tagged_PIF", new JArray(session, _vlan ?? ""), serializer);
        }

        public XenRef<PIF> vlan_get_untagged_pif(string session, string _vlan)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<PIF>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<PIF>>("VLAN.get_untagged_PIF", new JArray(session, _vlan ?? ""), serializer);
        }

        public long vlan_get_tag(string session, string _vlan)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<long>("VLAN.get_tag", new JArray(session, _vlan ?? ""), serializer);
        }

        public Dictionary<string, string> vlan_get_other_config(string session, string _vlan)
        {
            var converters = new List<JsonConverter> {new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<string, string>>("VLAN.get_other_config", new JArray(session, _vlan ?? ""), serializer);
        }

        public void vlan_set_other_config(string session, string _vlan, Dictionary<string, string> _other_config)
        {
            var converters = new List<JsonConverter> {new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            Rpc("VLAN.set_other_config", new JArray(session, _vlan ?? "", _other_config == null ? new JObject() : JObject.FromObject(_other_config, serializer)), serializer);
        }

        public void vlan_add_to_other_config(string session, string _vlan, string _key, string _value)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("VLAN.add_to_other_config", new JArray(session, _vlan ?? "", _key ?? "", _value ?? ""), serializer);
        }

        public void vlan_remove_from_other_config(string session, string _vlan, string _key)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("VLAN.remove_from_other_config", new JArray(session, _vlan ?? "", _key ?? ""), serializer);
        }

        public XenRef<VLAN> vlan_create(string session, string _tagged_pif, long _tag, string _network)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<VLAN>(), new XenRefConverter<PIF>(), new XenRefConverter<Network>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<VLAN>>("VLAN.create", new JArray(session, _tagged_pif ?? "", _tag, _network ?? ""), serializer);
        }

        public XenRef<Task> async_vlan_create(string session, string _tagged_pif, long _tag, string _network)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>(), new XenRefConverter<PIF>(), new XenRefConverter<Network>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.VLAN.create", new JArray(session, _tagged_pif ?? "", _tag, _network ?? ""), serializer);
        }

        public void vlan_destroy(string session, string _vlan)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("VLAN.destroy", new JArray(session, _vlan ?? ""), serializer);
        }

        public XenRef<Task> async_vlan_destroy(string session, string _vlan)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.VLAN.destroy", new JArray(session, _vlan ?? ""), serializer);
        }

        public List<XenRef<VLAN>> vlan_get_all(string session)
        {
            var converters = new List<JsonConverter> {new XenRefListConverter<VLAN>()};
            var serializer = CreateSerializer(converters);
            return Rpc<List<XenRef<VLAN>>>("VLAN.get_all", new JArray(session), serializer);
        }

        public Dictionary<XenRef<VLAN>, VLAN> vlan_get_all_records(string session)
        {
            var converters = new List<JsonConverter> {new XenRefXenObjectMapConverter<VLAN>()};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<XenRef<VLAN>, VLAN>>("VLAN.get_all_records", new JArray(session), serializer);
        }

        public SM sm_get_record(string session, string _sm)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<SM>("SM.get_record", new JArray(session, _sm ?? ""), serializer);
        }

        public XenRef<SM> sm_get_by_uuid(string session, string _uuid)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<SM>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<SM>>("SM.get_by_uuid", new JArray(session, _uuid ?? ""), serializer);
        }

        public List<XenRef<SM>> sm_get_by_name_label(string session, string _label)
        {
            var converters = new List<JsonConverter> {new XenRefListConverter<SM>()};
            var serializer = CreateSerializer(converters);
            return Rpc<List<XenRef<SM>>>("SM.get_by_name_label", new JArray(session, _label ?? ""), serializer);
        }

        public string sm_get_uuid(string session, string _sm)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("SM.get_uuid", new JArray(session, _sm ?? ""), serializer);
        }

        public string sm_get_name_label(string session, string _sm)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("SM.get_name_label", new JArray(session, _sm ?? ""), serializer);
        }

        public string sm_get_name_description(string session, string _sm)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("SM.get_name_description", new JArray(session, _sm ?? ""), serializer);
        }

        public string sm_get_type(string session, string _sm)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("SM.get_type", new JArray(session, _sm ?? ""), serializer);
        }

        public string sm_get_vendor(string session, string _sm)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("SM.get_vendor", new JArray(session, _sm ?? ""), serializer);
        }

        public string sm_get_copyright(string session, string _sm)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("SM.get_copyright", new JArray(session, _sm ?? ""), serializer);
        }

        public string sm_get_version(string session, string _sm)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("SM.get_version", new JArray(session, _sm ?? ""), serializer);
        }

        public string sm_get_required_api_version(string session, string _sm)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("SM.get_required_api_version", new JArray(session, _sm ?? ""), serializer);
        }

        public Dictionary<string, string> sm_get_configuration(string session, string _sm)
        {
            var converters = new List<JsonConverter> {new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<string, string>>("SM.get_configuration", new JArray(session, _sm ?? ""), serializer);
        }

        public string[] sm_get_capabilities(string session, string _sm)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string[]>("SM.get_capabilities", new JArray(session, _sm ?? ""), serializer);
        }

        public Dictionary<string, long> sm_get_features(string session, string _sm)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<string, long>>("SM.get_features", new JArray(session, _sm ?? ""), serializer);
        }

        public Dictionary<string, string> sm_get_other_config(string session, string _sm)
        {
            var converters = new List<JsonConverter> {new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<string, string>>("SM.get_other_config", new JArray(session, _sm ?? ""), serializer);
        }

        public string sm_get_driver_filename(string session, string _sm)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("SM.get_driver_filename", new JArray(session, _sm ?? ""), serializer);
        }

        public string[] sm_get_required_cluster_stack(string session, string _sm)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string[]>("SM.get_required_cluster_stack", new JArray(session, _sm ?? ""), serializer);
        }

        public void sm_set_other_config(string session, string _sm, Dictionary<string, string> _other_config)
        {
            var converters = new List<JsonConverter> {new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            Rpc("SM.set_other_config", new JArray(session, _sm ?? "", _other_config == null ? new JObject() : JObject.FromObject(_other_config, serializer)), serializer);
        }

        public void sm_add_to_other_config(string session, string _sm, string _key, string _value)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("SM.add_to_other_config", new JArray(session, _sm ?? "", _key ?? "", _value ?? ""), serializer);
        }

        public void sm_remove_from_other_config(string session, string _sm, string _key)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("SM.remove_from_other_config", new JArray(session, _sm ?? "", _key ?? ""), serializer);
        }

        public List<XenRef<SM>> sm_get_all(string session)
        {
            var converters = new List<JsonConverter> {new XenRefListConverter<SM>()};
            var serializer = CreateSerializer(converters);
            return Rpc<List<XenRef<SM>>>("SM.get_all", new JArray(session), serializer);
        }

        public Dictionary<XenRef<SM>, SM> sm_get_all_records(string session)
        {
            var converters = new List<JsonConverter> {new XenRefXenObjectMapConverter<SM>()};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<XenRef<SM>, SM>>("SM.get_all_records", new JArray(session), serializer);
        }

        public SR sr_get_record(string session, string _sr)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<SR>("SR.get_record", new JArray(session, _sr ?? ""), serializer);
        }

        public XenRef<SR> sr_get_by_uuid(string session, string _uuid)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<SR>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<SR>>("SR.get_by_uuid", new JArray(session, _uuid ?? ""), serializer);
        }

        public List<XenRef<SR>> sr_get_by_name_label(string session, string _label)
        {
            var converters = new List<JsonConverter> {new XenRefListConverter<SR>()};
            var serializer = CreateSerializer(converters);
            return Rpc<List<XenRef<SR>>>("SR.get_by_name_label", new JArray(session, _label ?? ""), serializer);
        }

        public string sr_get_uuid(string session, string _sr)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("SR.get_uuid", new JArray(session, _sr ?? ""), serializer);
        }

        public string sr_get_name_label(string session, string _sr)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("SR.get_name_label", new JArray(session, _sr ?? ""), serializer);
        }

        public string sr_get_name_description(string session, string _sr)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("SR.get_name_description", new JArray(session, _sr ?? ""), serializer);
        }

        public List<storage_operations> sr_get_allowed_operations(string session, string _sr)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<List<storage_operations>>("SR.get_allowed_operations", new JArray(session, _sr ?? ""), serializer);
        }

        public Dictionary<string, storage_operations> sr_get_current_operations(string session, string _sr)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<string, storage_operations>>("SR.get_current_operations", new JArray(session, _sr ?? ""), serializer);
        }

        public List<XenRef<VDI>> sr_get_vdis(string session, string _sr)
        {
            var converters = new List<JsonConverter> {new XenRefListConverter<VDI>()};
            var serializer = CreateSerializer(converters);
            return Rpc<List<XenRef<VDI>>>("SR.get_VDIs", new JArray(session, _sr ?? ""), serializer);
        }

        public List<XenRef<PBD>> sr_get_pbds(string session, string _sr)
        {
            var converters = new List<JsonConverter> {new XenRefListConverter<PBD>()};
            var serializer = CreateSerializer(converters);
            return Rpc<List<XenRef<PBD>>>("SR.get_PBDs", new JArray(session, _sr ?? ""), serializer);
        }

        public long sr_get_virtual_allocation(string session, string _sr)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<long>("SR.get_virtual_allocation", new JArray(session, _sr ?? ""), serializer);
        }

        public long sr_get_physical_utilisation(string session, string _sr)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<long>("SR.get_physical_utilisation", new JArray(session, _sr ?? ""), serializer);
        }

        public long sr_get_physical_size(string session, string _sr)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<long>("SR.get_physical_size", new JArray(session, _sr ?? ""), serializer);
        }

        public string sr_get_type(string session, string _sr)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("SR.get_type", new JArray(session, _sr ?? ""), serializer);
        }

        public string sr_get_content_type(string session, string _sr)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("SR.get_content_type", new JArray(session, _sr ?? ""), serializer);
        }

        public bool sr_get_shared(string session, string _sr)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<bool>("SR.get_shared", new JArray(session, _sr ?? ""), serializer);
        }

        public Dictionary<string, string> sr_get_other_config(string session, string _sr)
        {
            var converters = new List<JsonConverter> {new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<string, string>>("SR.get_other_config", new JArray(session, _sr ?? ""), serializer);
        }

        public string[] sr_get_tags(string session, string _sr)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string[]>("SR.get_tags", new JArray(session, _sr ?? ""), serializer);
        }

        public Dictionary<string, string> sr_get_sm_config(string session, string _sr)
        {
            var converters = new List<JsonConverter> {new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<string, string>>("SR.get_sm_config", new JArray(session, _sr ?? ""), serializer);
        }

        public Dictionary<string, XenRef<Blob>> sr_get_blobs(string session, string _sr)
        {
            var converters = new List<JsonConverter> {new StringXenRefMapConverter<Blob>()};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<string, XenRef<Blob>>>("SR.get_blobs", new JArray(session, _sr ?? ""), serializer);
        }

        public bool sr_get_local_cache_enabled(string session, string _sr)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<bool>("SR.get_local_cache_enabled", new JArray(session, _sr ?? ""), serializer);
        }

        public XenRef<DR_task> sr_get_introduced_by(string session, string _sr)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<DR_task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<DR_task>>("SR.get_introduced_by", new JArray(session, _sr ?? ""), serializer);
        }

        public bool sr_get_clustered(string session, string _sr)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<bool>("SR.get_clustered", new JArray(session, _sr ?? ""), serializer);
        }

        public bool sr_get_is_tools_sr(string session, string _sr)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<bool>("SR.get_is_tools_sr", new JArray(session, _sr ?? ""), serializer);
        }

        public void sr_set_other_config(string session, string _sr, Dictionary<string, string> _other_config)
        {
            var converters = new List<JsonConverter> {new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            Rpc("SR.set_other_config", new JArray(session, _sr ?? "", _other_config == null ? new JObject() : JObject.FromObject(_other_config, serializer)), serializer);
        }

        public void sr_add_to_other_config(string session, string _sr, string _key, string _value)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("SR.add_to_other_config", new JArray(session, _sr ?? "", _key ?? "", _value ?? ""), serializer);
        }

        public void sr_remove_from_other_config(string session, string _sr, string _key)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("SR.remove_from_other_config", new JArray(session, _sr ?? "", _key ?? ""), serializer);
        }

        public void sr_set_tags(string session, string _sr, string[] _tags)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("SR.set_tags", new JArray(session, _sr ?? "", _tags == null ? new JArray() : JArray.FromObject(_tags)), serializer);
        }

        public void sr_add_tags(string session, string _sr, string _value)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("SR.add_tags", new JArray(session, _sr ?? "", _value ?? ""), serializer);
        }

        public void sr_remove_tags(string session, string _sr, string _value)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("SR.remove_tags", new JArray(session, _sr ?? "", _value ?? ""), serializer);
        }

        public void sr_set_sm_config(string session, string _sr, Dictionary<string, string> _sm_config)
        {
            var converters = new List<JsonConverter> {new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            Rpc("SR.set_sm_config", new JArray(session, _sr ?? "", _sm_config == null ? new JObject() : JObject.FromObject(_sm_config, serializer)), serializer);
        }

        public void sr_add_to_sm_config(string session, string _sr, string _key, string _value)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("SR.add_to_sm_config", new JArray(session, _sr ?? "", _key ?? "", _value ?? ""), serializer);
        }

        public void sr_remove_from_sm_config(string session, string _sr, string _key)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("SR.remove_from_sm_config", new JArray(session, _sr ?? "", _key ?? ""), serializer);
        }

        public XenRef<SR> sr_create(string session, string _host, Dictionary<string, string> _device_config, long _physical_size, string _name_label, string _name_description, string _type, string _content_type, bool _shared)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<SR>(), new XenRefConverter<Host>(), new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<SR>>("SR.create", new JArray(session, _host ?? "", _device_config == null ? new JObject() : JObject.FromObject(_device_config, serializer), _physical_size, _name_label ?? "", _name_description ?? "", _type ?? "", _content_type ?? "", _shared), serializer);
        }

        public XenRef<Task> async_sr_create(string session, string _host, Dictionary<string, string> _device_config, long _physical_size, string _name_label, string _name_description, string _type, string _content_type, bool _shared)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>(), new XenRefConverter<Host>(), new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.SR.create", new JArray(session, _host ?? "", _device_config == null ? new JObject() : JObject.FromObject(_device_config, serializer), _physical_size, _name_label ?? "", _name_description ?? "", _type ?? "", _content_type ?? "", _shared), serializer);
        }

        public XenRef<SR> sr_create(string session, string _host, Dictionary<string, string> _device_config, long _physical_size, string _name_label, string _name_description, string _type, string _content_type, bool _shared, Dictionary<string, string> _sm_config)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<SR>(), new XenRefConverter<Host>(), new StringStringMapConverter(), new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<SR>>("SR.create", new JArray(session, _host ?? "", _device_config == null ? new JObject() : JObject.FromObject(_device_config, serializer), _physical_size, _name_label ?? "", _name_description ?? "", _type ?? "", _content_type ?? "", _shared, _sm_config == null ? new JObject() : JObject.FromObject(_sm_config, serializer)), serializer);
        }

        public XenRef<Task> async_sr_create(string session, string _host, Dictionary<string, string> _device_config, long _physical_size, string _name_label, string _name_description, string _type, string _content_type, bool _shared, Dictionary<string, string> _sm_config)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>(), new XenRefConverter<Host>(), new StringStringMapConverter(), new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.SR.create", new JArray(session, _host ?? "", _device_config == null ? new JObject() : JObject.FromObject(_device_config, serializer), _physical_size, _name_label ?? "", _name_description ?? "", _type ?? "", _content_type ?? "", _shared, _sm_config == null ? new JObject() : JObject.FromObject(_sm_config, serializer)), serializer);
        }

        public XenRef<SR> sr_introduce(string session, string _uuid, string _name_label, string _name_description, string _type, string _content_type, bool _shared)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<SR>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<SR>>("SR.introduce", new JArray(session, _uuid ?? "", _name_label ?? "", _name_description ?? "", _type ?? "", _content_type ?? "", _shared), serializer);
        }

        public XenRef<Task> async_sr_introduce(string session, string _uuid, string _name_label, string _name_description, string _type, string _content_type, bool _shared)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.SR.introduce", new JArray(session, _uuid ?? "", _name_label ?? "", _name_description ?? "", _type ?? "", _content_type ?? "", _shared), serializer);
        }

        public XenRef<SR> sr_introduce(string session, string _uuid, string _name_label, string _name_description, string _type, string _content_type, bool _shared, Dictionary<string, string> _sm_config)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<SR>(), new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<SR>>("SR.introduce", new JArray(session, _uuid ?? "", _name_label ?? "", _name_description ?? "", _type ?? "", _content_type ?? "", _shared, _sm_config == null ? new JObject() : JObject.FromObject(_sm_config, serializer)), serializer);
        }

        public XenRef<Task> async_sr_introduce(string session, string _uuid, string _name_label, string _name_description, string _type, string _content_type, bool _shared, Dictionary<string, string> _sm_config)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>(), new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.SR.introduce", new JArray(session, _uuid ?? "", _name_label ?? "", _name_description ?? "", _type ?? "", _content_type ?? "", _shared, _sm_config == null ? new JObject() : JObject.FromObject(_sm_config, serializer)), serializer);
        }

        public string sr_make(string session, string _host, Dictionary<string, string> _device_config, long _physical_size, string _name_label, string _name_description, string _type, string _content_type)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Host>(), new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("SR.make", new JArray(session, _host ?? "", _device_config == null ? new JObject() : JObject.FromObject(_device_config, serializer), _physical_size, _name_label ?? "", _name_description ?? "", _type ?? "", _content_type ?? ""), serializer);
        }

        public XenRef<Task> async_sr_make(string session, string _host, Dictionary<string, string> _device_config, long _physical_size, string _name_label, string _name_description, string _type, string _content_type)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>(), new XenRefConverter<Host>(), new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.SR.make", new JArray(session, _host ?? "", _device_config == null ? new JObject() : JObject.FromObject(_device_config, serializer), _physical_size, _name_label ?? "", _name_description ?? "", _type ?? "", _content_type ?? ""), serializer);
        }

        public string sr_make(string session, string _host, Dictionary<string, string> _device_config, long _physical_size, string _name_label, string _name_description, string _type, string _content_type, Dictionary<string, string> _sm_config)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Host>(), new StringStringMapConverter(), new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("SR.make", new JArray(session, _host ?? "", _device_config == null ? new JObject() : JObject.FromObject(_device_config, serializer), _physical_size, _name_label ?? "", _name_description ?? "", _type ?? "", _content_type ?? "", _sm_config == null ? new JObject() : JObject.FromObject(_sm_config, serializer)), serializer);
        }

        public XenRef<Task> async_sr_make(string session, string _host, Dictionary<string, string> _device_config, long _physical_size, string _name_label, string _name_description, string _type, string _content_type, Dictionary<string, string> _sm_config)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>(), new XenRefConverter<Host>(), new StringStringMapConverter(), new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.SR.make", new JArray(session, _host ?? "", _device_config == null ? new JObject() : JObject.FromObject(_device_config, serializer), _physical_size, _name_label ?? "", _name_description ?? "", _type ?? "", _content_type ?? "", _sm_config == null ? new JObject() : JObject.FromObject(_sm_config, serializer)), serializer);
        }

        public void sr_destroy(string session, string _sr)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("SR.destroy", new JArray(session, _sr ?? ""), serializer);
        }

        public XenRef<Task> async_sr_destroy(string session, string _sr)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.SR.destroy", new JArray(session, _sr ?? ""), serializer);
        }

        public void sr_forget(string session, string _sr)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("SR.forget", new JArray(session, _sr ?? ""), serializer);
        }

        public XenRef<Task> async_sr_forget(string session, string _sr)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.SR.forget", new JArray(session, _sr ?? ""), serializer);
        }

        public void sr_update(string session, string _sr)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("SR.update", new JArray(session, _sr ?? ""), serializer);
        }

        public XenRef<Task> async_sr_update(string session, string _sr)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.SR.update", new JArray(session, _sr ?? ""), serializer);
        }

        public string[] sr_get_supported_types(string session)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string[]>("SR.get_supported_types", new JArray(session), serializer);
        }

        public void sr_scan(string session, string _sr)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("SR.scan", new JArray(session, _sr ?? ""), serializer);
        }

        public XenRef<Task> async_sr_scan(string session, string _sr)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.SR.scan", new JArray(session, _sr ?? ""), serializer);
        }

        public string sr_probe(string session, string _host, Dictionary<string, string> _device_config, string _type, Dictionary<string, string> _sm_config)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Host>(), new StringStringMapConverter(), new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("SR.probe", new JArray(session, _host ?? "", _device_config == null ? new JObject() : JObject.FromObject(_device_config, serializer), _type ?? "", _sm_config == null ? new JObject() : JObject.FromObject(_sm_config, serializer)), serializer);
        }

        public XenRef<Task> async_sr_probe(string session, string _host, Dictionary<string, string> _device_config, string _type, Dictionary<string, string> _sm_config)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>(), new XenRefConverter<Host>(), new StringStringMapConverter(), new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.SR.probe", new JArray(session, _host ?? "", _device_config == null ? new JObject() : JObject.FromObject(_device_config, serializer), _type ?? "", _sm_config == null ? new JObject() : JObject.FromObject(_sm_config, serializer)), serializer);
        }

        public List<Probe_result> sr_probe_ext(string session, string _host, Dictionary<string, string> _device_config, string _type, Dictionary<string, string> _sm_config)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Host>(), new StringStringMapConverter(), new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<List<Probe_result>>("SR.probe_ext", new JArray(session, _host ?? "", _device_config == null ? new JObject() : JObject.FromObject(_device_config, serializer), _type ?? "", _sm_config == null ? new JObject() : JObject.FromObject(_sm_config, serializer)), serializer);
        }

        public XenRef<Task> async_sr_probe_ext(string session, string _host, Dictionary<string, string> _device_config, string _type, Dictionary<string, string> _sm_config)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>(), new XenRefConverter<Host>(), new StringStringMapConverter(), new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.SR.probe_ext", new JArray(session, _host ?? "", _device_config == null ? new JObject() : JObject.FromObject(_device_config, serializer), _type ?? "", _sm_config == null ? new JObject() : JObject.FromObject(_sm_config, serializer)), serializer);
        }

        public void sr_set_shared(string session, string _sr, bool _value)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("SR.set_shared", new JArray(session, _sr ?? "", _value), serializer);
        }

        public XenRef<Task> async_sr_set_shared(string session, string _sr, bool _value)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.SR.set_shared", new JArray(session, _sr ?? "", _value), serializer);
        }

        public void sr_set_name_label(string session, string _sr, string _value)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("SR.set_name_label", new JArray(session, _sr ?? "", _value ?? ""), serializer);
        }

        public XenRef<Task> async_sr_set_name_label(string session, string _sr, string _value)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.SR.set_name_label", new JArray(session, _sr ?? "", _value ?? ""), serializer);
        }

        public void sr_set_name_description(string session, string _sr, string _value)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("SR.set_name_description", new JArray(session, _sr ?? "", _value ?? ""), serializer);
        }

        public XenRef<Task> async_sr_set_name_description(string session, string _sr, string _value)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.SR.set_name_description", new JArray(session, _sr ?? "", _value ?? ""), serializer);
        }

        public XenRef<Blob> sr_create_new_blob(string session, string _sr, string _name, string _mime_type)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Blob>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Blob>>("SR.create_new_blob", new JArray(session, _sr ?? "", _name ?? "", _mime_type ?? ""), serializer);
        }

        public XenRef<Task> async_sr_create_new_blob(string session, string _sr, string _name, string _mime_type)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.SR.create_new_blob", new JArray(session, _sr ?? "", _name ?? "", _mime_type ?? ""), serializer);
        }

        public XenRef<Blob> sr_create_new_blob(string session, string _sr, string _name, string _mime_type, bool _public)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Blob>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Blob>>("SR.create_new_blob", new JArray(session, _sr ?? "", _name ?? "", _mime_type ?? "", _public), serializer);
        }

        public XenRef<Task> async_sr_create_new_blob(string session, string _sr, string _name, string _mime_type, bool _public)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.SR.create_new_blob", new JArray(session, _sr ?? "", _name ?? "", _mime_type ?? "", _public), serializer);
        }

        public void sr_set_physical_size(string session, string _sr, long _value)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("SR.set_physical_size", new JArray(session, _sr ?? "", _value), serializer);
        }

        public void sr_set_virtual_allocation(string session, string _sr, long _value)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("SR.set_virtual_allocation", new JArray(session, _sr ?? "", _value), serializer);
        }

        public void sr_set_physical_utilisation(string session, string _sr, long _value)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("SR.set_physical_utilisation", new JArray(session, _sr ?? "", _value), serializer);
        }

        public void sr_assert_can_host_ha_statefile(string session, string _sr)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("SR.assert_can_host_ha_statefile", new JArray(session, _sr ?? ""), serializer);
        }

        public XenRef<Task> async_sr_assert_can_host_ha_statefile(string session, string _sr)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.SR.assert_can_host_ha_statefile", new JArray(session, _sr ?? ""), serializer);
        }

        public void sr_assert_supports_database_replication(string session, string _sr)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("SR.assert_supports_database_replication", new JArray(session, _sr ?? ""), serializer);
        }

        public XenRef<Task> async_sr_assert_supports_database_replication(string session, string _sr)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.SR.assert_supports_database_replication", new JArray(session, _sr ?? ""), serializer);
        }

        public void sr_enable_database_replication(string session, string _sr)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("SR.enable_database_replication", new JArray(session, _sr ?? ""), serializer);
        }

        public XenRef<Task> async_sr_enable_database_replication(string session, string _sr)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.SR.enable_database_replication", new JArray(session, _sr ?? ""), serializer);
        }

        public void sr_disable_database_replication(string session, string _sr)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("SR.disable_database_replication", new JArray(session, _sr ?? ""), serializer);
        }

        public XenRef<Task> async_sr_disable_database_replication(string session, string _sr)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.SR.disable_database_replication", new JArray(session, _sr ?? ""), serializer);
        }

        public List<Data_source> sr_get_data_sources(string session, string _sr)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<List<Data_source>>("SR.get_data_sources", new JArray(session, _sr ?? ""), serializer);
        }

        public void sr_record_data_source(string session, string _sr, string _data_source)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("SR.record_data_source", new JArray(session, _sr ?? "", _data_source ?? ""), serializer);
        }

        public double sr_query_data_source(string session, string _sr, string _data_source)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<double>("SR.query_data_source", new JArray(session, _sr ?? "", _data_source ?? ""), serializer);
        }

        public void sr_forget_data_source_archives(string session, string _sr, string _data_source)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("SR.forget_data_source_archives", new JArray(session, _sr ?? "", _data_source ?? ""), serializer);
        }

        public List<XenRef<SR>> sr_get_all(string session)
        {
            var converters = new List<JsonConverter> {new XenRefListConverter<SR>()};
            var serializer = CreateSerializer(converters);
            return Rpc<List<XenRef<SR>>>("SR.get_all", new JArray(session), serializer);
        }

        public Dictionary<XenRef<SR>, SR> sr_get_all_records(string session)
        {
            var converters = new List<JsonConverter> {new XenRefXenObjectMapConverter<SR>()};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<XenRef<SR>, SR>>("SR.get_all_records", new JArray(session), serializer);
        }

        public Dictionary<XenRef<Sr_stat>, Sr_stat> sr_stat_get_all_records(string session)
        {
            var converters = new List<JsonConverter> {new XenRefXenObjectMapConverter<Sr_stat>()};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<XenRef<Sr_stat>, Sr_stat>>("sr_stat.get_all_records", new JArray(session), serializer);
        }

        public Dictionary<XenRef<Probe_result>, Probe_result> probe_result_get_all_records(string session)
        {
            var converters = new List<JsonConverter> {new XenRefXenObjectMapConverter<Probe_result>()};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<XenRef<Probe_result>, Probe_result>>("probe_result.get_all_records", new JArray(session), serializer);
        }

        public LVHD lvhd_get_record(string session, string _lvhd)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<LVHD>("LVHD.get_record", new JArray(session, _lvhd ?? ""), serializer);
        }

        public XenRef<LVHD> lvhd_get_by_uuid(string session, string _uuid)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<LVHD>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<LVHD>>("LVHD.get_by_uuid", new JArray(session, _uuid ?? ""), serializer);
        }

        public string lvhd_get_uuid(string session, string _lvhd)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("LVHD.get_uuid", new JArray(session, _lvhd ?? ""), serializer);
        }

        public string lvhd_enable_thin_provisioning(string session, string _host, string _sr, long _initial_allocation, long _allocation_quantum)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Host>(), new XenRefConverter<SR>()};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("LVHD.enable_thin_provisioning", new JArray(session, _host ?? "", _sr ?? "", _initial_allocation, _allocation_quantum), serializer);
        }

        public XenRef<Task> async_lvhd_enable_thin_provisioning(string session, string _host, string _sr, long _initial_allocation, long _allocation_quantum)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>(), new XenRefConverter<Host>(), new XenRefConverter<SR>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.LVHD.enable_thin_provisioning", new JArray(session, _host ?? "", _sr ?? "", _initial_allocation, _allocation_quantum), serializer);
        }

        public Dictionary<XenRef<LVHD>, LVHD> lvhd_get_all_records(string session)
        {
            var converters = new List<JsonConverter> {new XenRefXenObjectMapConverter<LVHD>()};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<XenRef<LVHD>, LVHD>>("LVHD.get_all_records", new JArray(session), serializer);
        }

        public VDI vdi_get_record(string session, string _vdi)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<VDI>("VDI.get_record", new JArray(session, _vdi ?? ""), serializer);
        }

        public XenRef<VDI> vdi_get_by_uuid(string session, string _uuid)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<VDI>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<VDI>>("VDI.get_by_uuid", new JArray(session, _uuid ?? ""), serializer);
        }

        public XenRef<VDI> vdi_create(string session, VDI _record)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<VDI>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<VDI>>("VDI.create", new JArray(session, _record.ToJObject()), serializer);
        }

        public XenRef<Task> async_vdi_create(string session, VDI _record)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.VDI.create", new JArray(session, _record.ToJObject()), serializer);
        }

        public void vdi_destroy(string session, string _vdi)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("VDI.destroy", new JArray(session, _vdi ?? ""), serializer);
        }

        public XenRef<Task> async_vdi_destroy(string session, string _vdi)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.VDI.destroy", new JArray(session, _vdi ?? ""), serializer);
        }

        public List<XenRef<VDI>> vdi_get_by_name_label(string session, string _label)
        {
            var converters = new List<JsonConverter> {new XenRefListConverter<VDI>()};
            var serializer = CreateSerializer(converters);
            return Rpc<List<XenRef<VDI>>>("VDI.get_by_name_label", new JArray(session, _label ?? ""), serializer);
        }

        public string vdi_get_uuid(string session, string _vdi)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("VDI.get_uuid", new JArray(session, _vdi ?? ""), serializer);
        }

        public string vdi_get_name_label(string session, string _vdi)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("VDI.get_name_label", new JArray(session, _vdi ?? ""), serializer);
        }

        public string vdi_get_name_description(string session, string _vdi)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("VDI.get_name_description", new JArray(session, _vdi ?? ""), serializer);
        }

        public List<vdi_operations> vdi_get_allowed_operations(string session, string _vdi)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<List<vdi_operations>>("VDI.get_allowed_operations", new JArray(session, _vdi ?? ""), serializer);
        }

        public Dictionary<string, vdi_operations> vdi_get_current_operations(string session, string _vdi)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<string, vdi_operations>>("VDI.get_current_operations", new JArray(session, _vdi ?? ""), serializer);
        }

        public XenRef<SR> vdi_get_sr(string session, string _vdi)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<SR>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<SR>>("VDI.get_SR", new JArray(session, _vdi ?? ""), serializer);
        }

        public List<XenRef<VBD>> vdi_get_vbds(string session, string _vdi)
        {
            var converters = new List<JsonConverter> {new XenRefListConverter<VBD>()};
            var serializer = CreateSerializer(converters);
            return Rpc<List<XenRef<VBD>>>("VDI.get_VBDs", new JArray(session, _vdi ?? ""), serializer);
        }

        public List<XenRef<Crashdump>> vdi_get_crash_dumps(string session, string _vdi)
        {
            var converters = new List<JsonConverter> {new XenRefListConverter<Crashdump>()};
            var serializer = CreateSerializer(converters);
            return Rpc<List<XenRef<Crashdump>>>("VDI.get_crash_dumps", new JArray(session, _vdi ?? ""), serializer);
        }

        public long vdi_get_virtual_size(string session, string _vdi)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<long>("VDI.get_virtual_size", new JArray(session, _vdi ?? ""), serializer);
        }

        public long vdi_get_physical_utilisation(string session, string _vdi)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<long>("VDI.get_physical_utilisation", new JArray(session, _vdi ?? ""), serializer);
        }

        public vdi_type vdi_get_type(string session, string _vdi)
        {
            var converters = new List<JsonConverter> {new vdi_typeConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<vdi_type>("VDI.get_type", new JArray(session, _vdi ?? ""), serializer);
        }

        public bool vdi_get_sharable(string session, string _vdi)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<bool>("VDI.get_sharable", new JArray(session, _vdi ?? ""), serializer);
        }

        public bool vdi_get_read_only(string session, string _vdi)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<bool>("VDI.get_read_only", new JArray(session, _vdi ?? ""), serializer);
        }

        public Dictionary<string, string> vdi_get_other_config(string session, string _vdi)
        {
            var converters = new List<JsonConverter> {new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<string, string>>("VDI.get_other_config", new JArray(session, _vdi ?? ""), serializer);
        }

        public bool vdi_get_storage_lock(string session, string _vdi)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<bool>("VDI.get_storage_lock", new JArray(session, _vdi ?? ""), serializer);
        }

        public string vdi_get_location(string session, string _vdi)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("VDI.get_location", new JArray(session, _vdi ?? ""), serializer);
        }

        public bool vdi_get_managed(string session, string _vdi)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<bool>("VDI.get_managed", new JArray(session, _vdi ?? ""), serializer);
        }

        public bool vdi_get_missing(string session, string _vdi)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<bool>("VDI.get_missing", new JArray(session, _vdi ?? ""), serializer);
        }

        public XenRef<VDI> vdi_get_parent(string session, string _vdi)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<VDI>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<VDI>>("VDI.get_parent", new JArray(session, _vdi ?? ""), serializer);
        }

        public Dictionary<string, string> vdi_get_xenstore_data(string session, string _vdi)
        {
            var converters = new List<JsonConverter> {new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<string, string>>("VDI.get_xenstore_data", new JArray(session, _vdi ?? ""), serializer);
        }

        public Dictionary<string, string> vdi_get_sm_config(string session, string _vdi)
        {
            var converters = new List<JsonConverter> {new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<string, string>>("VDI.get_sm_config", new JArray(session, _vdi ?? ""), serializer);
        }

        public bool vdi_get_is_a_snapshot(string session, string _vdi)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<bool>("VDI.get_is_a_snapshot", new JArray(session, _vdi ?? ""), serializer);
        }

        public XenRef<VDI> vdi_get_snapshot_of(string session, string _vdi)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<VDI>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<VDI>>("VDI.get_snapshot_of", new JArray(session, _vdi ?? ""), serializer);
        }

        public List<XenRef<VDI>> vdi_get_snapshots(string session, string _vdi)
        {
            var converters = new List<JsonConverter> {new XenRefListConverter<VDI>()};
            var serializer = CreateSerializer(converters);
            return Rpc<List<XenRef<VDI>>>("VDI.get_snapshots", new JArray(session, _vdi ?? ""), serializer);
        }

        public DateTime vdi_get_snapshot_time(string session, string _vdi)
        {
            var converters = new List<JsonConverter> {new XenDateTimeConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<DateTime>("VDI.get_snapshot_time", new JArray(session, _vdi ?? ""), serializer);
        }

        public string[] vdi_get_tags(string session, string _vdi)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string[]>("VDI.get_tags", new JArray(session, _vdi ?? ""), serializer);
        }

        public bool vdi_get_allow_caching(string session, string _vdi)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<bool>("VDI.get_allow_caching", new JArray(session, _vdi ?? ""), serializer);
        }

        public on_boot vdi_get_on_boot(string session, string _vdi)
        {
            var converters = new List<JsonConverter> {new on_bootConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<on_boot>("VDI.get_on_boot", new JArray(session, _vdi ?? ""), serializer);
        }

        public XenRef<Pool> vdi_get_metadata_of_pool(string session, string _vdi)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Pool>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Pool>>("VDI.get_metadata_of_pool", new JArray(session, _vdi ?? ""), serializer);
        }

        public bool vdi_get_metadata_latest(string session, string _vdi)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<bool>("VDI.get_metadata_latest", new JArray(session, _vdi ?? ""), serializer);
        }

        public bool vdi_get_is_tools_iso(string session, string _vdi)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<bool>("VDI.get_is_tools_iso", new JArray(session, _vdi ?? ""), serializer);
        }

        public bool vdi_get_cbt_enabled(string session, string _vdi)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<bool>("VDI.get_cbt_enabled", new JArray(session, _vdi ?? ""), serializer);
        }

        public void vdi_set_other_config(string session, string _vdi, Dictionary<string, string> _other_config)
        {
            var converters = new List<JsonConverter> {new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            Rpc("VDI.set_other_config", new JArray(session, _vdi ?? "", _other_config == null ? new JObject() : JObject.FromObject(_other_config, serializer)), serializer);
        }

        public void vdi_add_to_other_config(string session, string _vdi, string _key, string _value)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("VDI.add_to_other_config", new JArray(session, _vdi ?? "", _key ?? "", _value ?? ""), serializer);
        }

        public void vdi_remove_from_other_config(string session, string _vdi, string _key)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("VDI.remove_from_other_config", new JArray(session, _vdi ?? "", _key ?? ""), serializer);
        }

        public void vdi_set_xenstore_data(string session, string _vdi, Dictionary<string, string> _xenstore_data)
        {
            var converters = new List<JsonConverter> {new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            Rpc("VDI.set_xenstore_data", new JArray(session, _vdi ?? "", _xenstore_data == null ? new JObject() : JObject.FromObject(_xenstore_data, serializer)), serializer);
        }

        public void vdi_add_to_xenstore_data(string session, string _vdi, string _key, string _value)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("VDI.add_to_xenstore_data", new JArray(session, _vdi ?? "", _key ?? "", _value ?? ""), serializer);
        }

        public void vdi_remove_from_xenstore_data(string session, string _vdi, string _key)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("VDI.remove_from_xenstore_data", new JArray(session, _vdi ?? "", _key ?? ""), serializer);
        }

        public void vdi_set_sm_config(string session, string _vdi, Dictionary<string, string> _sm_config)
        {
            var converters = new List<JsonConverter> {new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            Rpc("VDI.set_sm_config", new JArray(session, _vdi ?? "", _sm_config == null ? new JObject() : JObject.FromObject(_sm_config, serializer)), serializer);
        }

        public void vdi_add_to_sm_config(string session, string _vdi, string _key, string _value)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("VDI.add_to_sm_config", new JArray(session, _vdi ?? "", _key ?? "", _value ?? ""), serializer);
        }

        public void vdi_remove_from_sm_config(string session, string _vdi, string _key)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("VDI.remove_from_sm_config", new JArray(session, _vdi ?? "", _key ?? ""), serializer);
        }

        public void vdi_set_tags(string session, string _vdi, string[] _tags)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("VDI.set_tags", new JArray(session, _vdi ?? "", _tags == null ? new JArray() : JArray.FromObject(_tags)), serializer);
        }

        public void vdi_add_tags(string session, string _vdi, string _value)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("VDI.add_tags", new JArray(session, _vdi ?? "", _value ?? ""), serializer);
        }

        public void vdi_remove_tags(string session, string _vdi, string _value)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("VDI.remove_tags", new JArray(session, _vdi ?? "", _value ?? ""), serializer);
        }

        public XenRef<VDI> vdi_snapshot(string session, string _vdi, Dictionary<string, string> _driver_params)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<VDI>(), new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<VDI>>("VDI.snapshot", new JArray(session, _vdi ?? "", _driver_params == null ? new JObject() : JObject.FromObject(_driver_params, serializer)), serializer);
        }

        public XenRef<Task> async_vdi_snapshot(string session, string _vdi, Dictionary<string, string> _driver_params)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>(), new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.VDI.snapshot", new JArray(session, _vdi ?? "", _driver_params == null ? new JObject() : JObject.FromObject(_driver_params, serializer)), serializer);
        }

        public XenRef<VDI> vdi_clone(string session, string _vdi, Dictionary<string, string> _driver_params)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<VDI>(), new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<VDI>>("VDI.clone", new JArray(session, _vdi ?? "", _driver_params == null ? new JObject() : JObject.FromObject(_driver_params, serializer)), serializer);
        }

        public XenRef<Task> async_vdi_clone(string session, string _vdi, Dictionary<string, string> _driver_params)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>(), new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.VDI.clone", new JArray(session, _vdi ?? "", _driver_params == null ? new JObject() : JObject.FromObject(_driver_params, serializer)), serializer);
        }

        public void vdi_resize(string session, string _vdi, long _size)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("VDI.resize", new JArray(session, _vdi ?? "", _size), serializer);
        }

        public XenRef<Task> async_vdi_resize(string session, string _vdi, long _size)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.VDI.resize", new JArray(session, _vdi ?? "", _size), serializer);
        }

        public void vdi_resize_online(string session, string _vdi, long _size)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("VDI.resize_online", new JArray(session, _vdi ?? "", _size), serializer);
        }

        public XenRef<Task> async_vdi_resize_online(string session, string _vdi, long _size)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.VDI.resize_online", new JArray(session, _vdi ?? "", _size), serializer);
        }

        public XenRef<VDI> vdi_introduce(string session, string _uuid, string _name_label, string _name_description, string _sr, vdi_type _type, bool _sharable, bool _read_only, Dictionary<string, string> _other_config, string _location, Dictionary<string, string> _xenstore_data, Dictionary<string, string> _sm_config)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<VDI>(), new XenRefConverter<SR>(), new vdi_typeConverter(), new StringStringMapConverter(), new StringStringMapConverter(), new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<VDI>>("VDI.introduce", new JArray(session, _uuid ?? "", _name_label ?? "", _name_description ?? "", _sr ?? "", _type.StringOf(), _sharable, _read_only, _other_config == null ? new JObject() : JObject.FromObject(_other_config, serializer), _location ?? "", _xenstore_data == null ? new JObject() : JObject.FromObject(_xenstore_data, serializer), _sm_config == null ? new JObject() : JObject.FromObject(_sm_config, serializer)), serializer);
        }

        public XenRef<Task> async_vdi_introduce(string session, string _uuid, string _name_label, string _name_description, string _sr, vdi_type _type, bool _sharable, bool _read_only, Dictionary<string, string> _other_config, string _location, Dictionary<string, string> _xenstore_data, Dictionary<string, string> _sm_config)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>(), new XenRefConverter<SR>(), new vdi_typeConverter(), new StringStringMapConverter(), new StringStringMapConverter(), new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.VDI.introduce", new JArray(session, _uuid ?? "", _name_label ?? "", _name_description ?? "", _sr ?? "", _type.StringOf(), _sharable, _read_only, _other_config == null ? new JObject() : JObject.FromObject(_other_config, serializer), _location ?? "", _xenstore_data == null ? new JObject() : JObject.FromObject(_xenstore_data, serializer), _sm_config == null ? new JObject() : JObject.FromObject(_sm_config, serializer)), serializer);
        }

        public XenRef<VDI> vdi_introduce(string session, string _uuid, string _name_label, string _name_description, string _sr, vdi_type _type, bool _sharable, bool _read_only, Dictionary<string, string> _other_config, string _location, Dictionary<string, string> _xenstore_data, Dictionary<string, string> _sm_config, bool _managed, long _virtual_size, long _physical_utilisation, string _metadata_of_pool, bool _is_a_snapshot, DateTime _snapshot_time, string _snapshot_of)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<VDI>(), new XenRefConverter<SR>(), new vdi_typeConverter(), new StringStringMapConverter(), new StringStringMapConverter(), new StringStringMapConverter(), new XenRefConverter<Pool>(), new XenDateTimeConverter(), new XenRefConverter<VDI>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<VDI>>("VDI.introduce", new JArray(session, _uuid ?? "", _name_label ?? "", _name_description ?? "", _sr ?? "", _type.StringOf(), _sharable, _read_only, _other_config == null ? new JObject() : JObject.FromObject(_other_config, serializer), _location ?? "", _xenstore_data == null ? new JObject() : JObject.FromObject(_xenstore_data, serializer), _sm_config == null ? new JObject() : JObject.FromObject(_sm_config, serializer), _managed, _virtual_size, _physical_utilisation, _metadata_of_pool ?? "", _is_a_snapshot, _snapshot_time, _snapshot_of ?? ""), serializer);
        }

        public XenRef<Task> async_vdi_introduce(string session, string _uuid, string _name_label, string _name_description, string _sr, vdi_type _type, bool _sharable, bool _read_only, Dictionary<string, string> _other_config, string _location, Dictionary<string, string> _xenstore_data, Dictionary<string, string> _sm_config, bool _managed, long _virtual_size, long _physical_utilisation, string _metadata_of_pool, bool _is_a_snapshot, DateTime _snapshot_time, string _snapshot_of)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>(), new XenRefConverter<SR>(), new vdi_typeConverter(), new StringStringMapConverter(), new StringStringMapConverter(), new StringStringMapConverter(), new XenRefConverter<Pool>(), new XenDateTimeConverter(), new XenRefConverter<VDI>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.VDI.introduce", new JArray(session, _uuid ?? "", _name_label ?? "", _name_description ?? "", _sr ?? "", _type.StringOf(), _sharable, _read_only, _other_config == null ? new JObject() : JObject.FromObject(_other_config, serializer), _location ?? "", _xenstore_data == null ? new JObject() : JObject.FromObject(_xenstore_data, serializer), _sm_config == null ? new JObject() : JObject.FromObject(_sm_config, serializer), _managed, _virtual_size, _physical_utilisation, _metadata_of_pool ?? "", _is_a_snapshot, _snapshot_time, _snapshot_of ?? ""), serializer);
        }

        public XenRef<VDI> vdi_db_introduce(string session, string _uuid, string _name_label, string _name_description, string _sr, vdi_type _type, bool _sharable, bool _read_only, Dictionary<string, string> _other_config, string _location, Dictionary<string, string> _xenstore_data, Dictionary<string, string> _sm_config)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<VDI>(), new XenRefConverter<SR>(), new vdi_typeConverter(), new StringStringMapConverter(), new StringStringMapConverter(), new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<VDI>>("VDI.db_introduce", new JArray(session, _uuid ?? "", _name_label ?? "", _name_description ?? "", _sr ?? "", _type.StringOf(), _sharable, _read_only, _other_config == null ? new JObject() : JObject.FromObject(_other_config, serializer), _location ?? "", _xenstore_data == null ? new JObject() : JObject.FromObject(_xenstore_data, serializer), _sm_config == null ? new JObject() : JObject.FromObject(_sm_config, serializer)), serializer);
        }

        public XenRef<Task> async_vdi_db_introduce(string session, string _uuid, string _name_label, string _name_description, string _sr, vdi_type _type, bool _sharable, bool _read_only, Dictionary<string, string> _other_config, string _location, Dictionary<string, string> _xenstore_data, Dictionary<string, string> _sm_config)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>(), new XenRefConverter<SR>(), new vdi_typeConverter(), new StringStringMapConverter(), new StringStringMapConverter(), new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.VDI.db_introduce", new JArray(session, _uuid ?? "", _name_label ?? "", _name_description ?? "", _sr ?? "", _type.StringOf(), _sharable, _read_only, _other_config == null ? new JObject() : JObject.FromObject(_other_config, serializer), _location ?? "", _xenstore_data == null ? new JObject() : JObject.FromObject(_xenstore_data, serializer), _sm_config == null ? new JObject() : JObject.FromObject(_sm_config, serializer)), serializer);
        }

        public XenRef<VDI> vdi_db_introduce(string session, string _uuid, string _name_label, string _name_description, string _sr, vdi_type _type, bool _sharable, bool _read_only, Dictionary<string, string> _other_config, string _location, Dictionary<string, string> _xenstore_data, Dictionary<string, string> _sm_config, bool _managed, long _virtual_size, long _physical_utilisation, string _metadata_of_pool, bool _is_a_snapshot, DateTime _snapshot_time, string _snapshot_of)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<VDI>(), new XenRefConverter<SR>(), new vdi_typeConverter(), new StringStringMapConverter(), new StringStringMapConverter(), new StringStringMapConverter(), new XenRefConverter<Pool>(), new XenDateTimeConverter(), new XenRefConverter<VDI>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<VDI>>("VDI.db_introduce", new JArray(session, _uuid ?? "", _name_label ?? "", _name_description ?? "", _sr ?? "", _type.StringOf(), _sharable, _read_only, _other_config == null ? new JObject() : JObject.FromObject(_other_config, serializer), _location ?? "", _xenstore_data == null ? new JObject() : JObject.FromObject(_xenstore_data, serializer), _sm_config == null ? new JObject() : JObject.FromObject(_sm_config, serializer), _managed, _virtual_size, _physical_utilisation, _metadata_of_pool ?? "", _is_a_snapshot, _snapshot_time, _snapshot_of ?? ""), serializer);
        }

        public XenRef<Task> async_vdi_db_introduce(string session, string _uuid, string _name_label, string _name_description, string _sr, vdi_type _type, bool _sharable, bool _read_only, Dictionary<string, string> _other_config, string _location, Dictionary<string, string> _xenstore_data, Dictionary<string, string> _sm_config, bool _managed, long _virtual_size, long _physical_utilisation, string _metadata_of_pool, bool _is_a_snapshot, DateTime _snapshot_time, string _snapshot_of)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>(), new XenRefConverter<SR>(), new vdi_typeConverter(), new StringStringMapConverter(), new StringStringMapConverter(), new StringStringMapConverter(), new XenRefConverter<Pool>(), new XenDateTimeConverter(), new XenRefConverter<VDI>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.VDI.db_introduce", new JArray(session, _uuid ?? "", _name_label ?? "", _name_description ?? "", _sr ?? "", _type.StringOf(), _sharable, _read_only, _other_config == null ? new JObject() : JObject.FromObject(_other_config, serializer), _location ?? "", _xenstore_data == null ? new JObject() : JObject.FromObject(_xenstore_data, serializer), _sm_config == null ? new JObject() : JObject.FromObject(_sm_config, serializer), _managed, _virtual_size, _physical_utilisation, _metadata_of_pool ?? "", _is_a_snapshot, _snapshot_time, _snapshot_of ?? ""), serializer);
        }

        public XenRef<VDI> vdi_db_introduce(string session, string _uuid, string _name_label, string _name_description, string _sr, vdi_type _type, bool _sharable, bool _read_only, Dictionary<string, string> _other_config, string _location, Dictionary<string, string> _xenstore_data, Dictionary<string, string> _sm_config, bool _managed, long _virtual_size, long _physical_utilisation, string _metadata_of_pool, bool _is_a_snapshot, DateTime _snapshot_time, string _snapshot_of, bool _cbt_enabled)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<VDI>(), new XenRefConverter<SR>(), new vdi_typeConverter(), new StringStringMapConverter(), new StringStringMapConverter(), new StringStringMapConverter(), new XenRefConverter<Pool>(), new XenDateTimeConverter(), new XenRefConverter<VDI>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<VDI>>("VDI.db_introduce", new JArray(session, _uuid ?? "", _name_label ?? "", _name_description ?? "", _sr ?? "", _type.StringOf(), _sharable, _read_only, _other_config == null ? new JObject() : JObject.FromObject(_other_config, serializer), _location ?? "", _xenstore_data == null ? new JObject() : JObject.FromObject(_xenstore_data, serializer), _sm_config == null ? new JObject() : JObject.FromObject(_sm_config, serializer), _managed, _virtual_size, _physical_utilisation, _metadata_of_pool ?? "", _is_a_snapshot, _snapshot_time, _snapshot_of ?? "", _cbt_enabled), serializer);
        }

        public XenRef<Task> async_vdi_db_introduce(string session, string _uuid, string _name_label, string _name_description, string _sr, vdi_type _type, bool _sharable, bool _read_only, Dictionary<string, string> _other_config, string _location, Dictionary<string, string> _xenstore_data, Dictionary<string, string> _sm_config, bool _managed, long _virtual_size, long _physical_utilisation, string _metadata_of_pool, bool _is_a_snapshot, DateTime _snapshot_time, string _snapshot_of, bool _cbt_enabled)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>(), new XenRefConverter<SR>(), new vdi_typeConverter(), new StringStringMapConverter(), new StringStringMapConverter(), new StringStringMapConverter(), new XenRefConverter<Pool>(), new XenDateTimeConverter(), new XenRefConverter<VDI>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.VDI.db_introduce", new JArray(session, _uuid ?? "", _name_label ?? "", _name_description ?? "", _sr ?? "", _type.StringOf(), _sharable, _read_only, _other_config == null ? new JObject() : JObject.FromObject(_other_config, serializer), _location ?? "", _xenstore_data == null ? new JObject() : JObject.FromObject(_xenstore_data, serializer), _sm_config == null ? new JObject() : JObject.FromObject(_sm_config, serializer), _managed, _virtual_size, _physical_utilisation, _metadata_of_pool ?? "", _is_a_snapshot, _snapshot_time, _snapshot_of ?? "", _cbt_enabled), serializer);
        }

        public void vdi_db_forget(string session, string _vdi)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("VDI.db_forget", new JArray(session, _vdi ?? ""), serializer);
        }

        public XenRef<Task> async_vdi_db_forget(string session, string _vdi)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.VDI.db_forget", new JArray(session, _vdi ?? ""), serializer);
        }

        public void vdi_update(string session, string _vdi)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("VDI.update", new JArray(session, _vdi ?? ""), serializer);
        }

        public XenRef<Task> async_vdi_update(string session, string _vdi)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.VDI.update", new JArray(session, _vdi ?? ""), serializer);
        }

        public XenRef<VDI> vdi_copy(string session, string _vdi, string _sr)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<VDI>(), new XenRefConverter<SR>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<VDI>>("VDI.copy", new JArray(session, _vdi ?? "", _sr ?? ""), serializer);
        }

        public XenRef<Task> async_vdi_copy(string session, string _vdi, string _sr)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>(), new XenRefConverter<SR>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.VDI.copy", new JArray(session, _vdi ?? "", _sr ?? ""), serializer);
        }

        public XenRef<VDI> vdi_copy(string session, string _vdi, string _sr, string _base_vdi, string _into_vdi)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<VDI>(), new XenRefConverter<SR>(), new XenRefConverter<VDI>(), new XenRefConverter<VDI>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<VDI>>("VDI.copy", new JArray(session, _vdi ?? "", _sr ?? "", _base_vdi ?? "", _into_vdi ?? ""), serializer);
        }

        public XenRef<Task> async_vdi_copy(string session, string _vdi, string _sr, string _base_vdi, string _into_vdi)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>(), new XenRefConverter<SR>(), new XenRefConverter<VDI>(), new XenRefConverter<VDI>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.VDI.copy", new JArray(session, _vdi ?? "", _sr ?? "", _base_vdi ?? "", _into_vdi ?? ""), serializer);
        }

        public void vdi_set_managed(string session, string _vdi, bool _value)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("VDI.set_managed", new JArray(session, _vdi ?? "", _value), serializer);
        }

        public void vdi_forget(string session, string _vdi)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("VDI.forget", new JArray(session, _vdi ?? ""), serializer);
        }

        public XenRef<Task> async_vdi_forget(string session, string _vdi)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.VDI.forget", new JArray(session, _vdi ?? ""), serializer);
        }

        public void vdi_set_sharable(string session, string _vdi, bool _value)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("VDI.set_sharable", new JArray(session, _vdi ?? "", _value), serializer);
        }

        public void vdi_set_read_only(string session, string _vdi, bool _value)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("VDI.set_read_only", new JArray(session, _vdi ?? "", _value), serializer);
        }

        public void vdi_set_missing(string session, string _vdi, bool _value)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("VDI.set_missing", new JArray(session, _vdi ?? "", _value), serializer);
        }

        public void vdi_set_virtual_size(string session, string _vdi, long _value)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("VDI.set_virtual_size", new JArray(session, _vdi ?? "", _value), serializer);
        }

        public void vdi_set_physical_utilisation(string session, string _vdi, long _value)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("VDI.set_physical_utilisation", new JArray(session, _vdi ?? "", _value), serializer);
        }

        public void vdi_set_is_a_snapshot(string session, string _vdi, bool _value)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("VDI.set_is_a_snapshot", new JArray(session, _vdi ?? "", _value), serializer);
        }

        public void vdi_set_snapshot_of(string session, string _vdi, string _value)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<VDI>()};
            var serializer = CreateSerializer(converters);
            Rpc("VDI.set_snapshot_of", new JArray(session, _vdi ?? "", _value ?? ""), serializer);
        }

        public void vdi_set_snapshot_time(string session, string _vdi, DateTime _value)
        {
            var converters = new List<JsonConverter> {new XenDateTimeConverter()};
            var serializer = CreateSerializer(converters);
            Rpc("VDI.set_snapshot_time", new JArray(session, _vdi ?? "", _value), serializer);
        }

        public void vdi_set_metadata_of_pool(string session, string _vdi, string _value)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Pool>()};
            var serializer = CreateSerializer(converters);
            Rpc("VDI.set_metadata_of_pool", new JArray(session, _vdi ?? "", _value ?? ""), serializer);
        }

        public void vdi_set_name_label(string session, string _vdi, string _value)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("VDI.set_name_label", new JArray(session, _vdi ?? "", _value ?? ""), serializer);
        }

        public XenRef<Task> async_vdi_set_name_label(string session, string _vdi, string _value)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.VDI.set_name_label", new JArray(session, _vdi ?? "", _value ?? ""), serializer);
        }

        public void vdi_set_name_description(string session, string _vdi, string _value)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("VDI.set_name_description", new JArray(session, _vdi ?? "", _value ?? ""), serializer);
        }

        public XenRef<Task> async_vdi_set_name_description(string session, string _vdi, string _value)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.VDI.set_name_description", new JArray(session, _vdi ?? "", _value ?? ""), serializer);
        }

        public void vdi_set_on_boot(string session, string _vdi, on_boot _value)
        {
            var converters = new List<JsonConverter> {new on_bootConverter()};
            var serializer = CreateSerializer(converters);
            Rpc("VDI.set_on_boot", new JArray(session, _vdi ?? "", _value.StringOf()), serializer);
        }

        public XenRef<Task> async_vdi_set_on_boot(string session, string _vdi, on_boot _value)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>(), new on_bootConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.VDI.set_on_boot", new JArray(session, _vdi ?? "", _value.StringOf()), serializer);
        }

        public void vdi_set_allow_caching(string session, string _vdi, bool _value)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("VDI.set_allow_caching", new JArray(session, _vdi ?? "", _value), serializer);
        }

        public XenRef<Task> async_vdi_set_allow_caching(string session, string _vdi, bool _value)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.VDI.set_allow_caching", new JArray(session, _vdi ?? "", _value), serializer);
        }

        public XenRef<Session> vdi_open_database(string session, string _vdi)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Session>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Session>>("VDI.open_database", new JArray(session, _vdi ?? ""), serializer);
        }

        public XenRef<Task> async_vdi_open_database(string session, string _vdi)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.VDI.open_database", new JArray(session, _vdi ?? ""), serializer);
        }

        public string vdi_read_database_pool_uuid(string session, string _vdi)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("VDI.read_database_pool_uuid", new JArray(session, _vdi ?? ""), serializer);
        }

        public XenRef<Task> async_vdi_read_database_pool_uuid(string session, string _vdi)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.VDI.read_database_pool_uuid", new JArray(session, _vdi ?? ""), serializer);
        }

        public XenRef<VDI> vdi_pool_migrate(string session, string _vdi, string _sr, Dictionary<string, string> _options)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<VDI>(), new XenRefConverter<SR>(), new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<VDI>>("VDI.pool_migrate", new JArray(session, _vdi ?? "", _sr ?? "", _options == null ? new JObject() : JObject.FromObject(_options, serializer)), serializer);
        }

        public XenRef<Task> async_vdi_pool_migrate(string session, string _vdi, string _sr, Dictionary<string, string> _options)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>(), new XenRefConverter<SR>(), new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.VDI.pool_migrate", new JArray(session, _vdi ?? "", _sr ?? "", _options == null ? new JObject() : JObject.FromObject(_options, serializer)), serializer);
        }

        public void vdi_enable_cbt(string session, string _vdi)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("VDI.enable_cbt", new JArray(session, _vdi ?? ""), serializer);
        }

        public XenRef<Task> async_vdi_enable_cbt(string session, string _vdi)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.VDI.enable_cbt", new JArray(session, _vdi ?? ""), serializer);
        }

        public void vdi_disable_cbt(string session, string _vdi)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("VDI.disable_cbt", new JArray(session, _vdi ?? ""), serializer);
        }

        public XenRef<Task> async_vdi_disable_cbt(string session, string _vdi)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.VDI.disable_cbt", new JArray(session, _vdi ?? ""), serializer);
        }

        public void vdi_data_destroy(string session, string _vdi)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("VDI.data_destroy", new JArray(session, _vdi ?? ""), serializer);
        }

        public XenRef<Task> async_vdi_data_destroy(string session, string _vdi)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.VDI.data_destroy", new JArray(session, _vdi ?? ""), serializer);
        }

        public string vdi_list_changed_blocks(string session, string _vdi, string _vdi_to)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<VDI>()};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("VDI.list_changed_blocks", new JArray(session, _vdi ?? "", _vdi_to ?? ""), serializer);
        }

        public XenRef<Task> async_vdi_list_changed_blocks(string session, string _vdi, string _vdi_to)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>(), new XenRefConverter<VDI>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.VDI.list_changed_blocks", new JArray(session, _vdi ?? "", _vdi_to ?? ""), serializer);
        }

        public List<Vdi_nbd_server_info> vdi_get_nbd_info(string session, string _vdi)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<List<Vdi_nbd_server_info>>("VDI.get_nbd_info", new JArray(session, _vdi ?? ""), serializer);
        }

        public List<XenRef<VDI>> vdi_get_all(string session)
        {
            var converters = new List<JsonConverter> {new XenRefListConverter<VDI>()};
            var serializer = CreateSerializer(converters);
            return Rpc<List<XenRef<VDI>>>("VDI.get_all", new JArray(session), serializer);
        }

        public Dictionary<XenRef<VDI>, VDI> vdi_get_all_records(string session)
        {
            var converters = new List<JsonConverter> {new XenRefXenObjectMapConverter<VDI>()};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<XenRef<VDI>, VDI>>("VDI.get_all_records", new JArray(session), serializer);
        }

        public VBD vbd_get_record(string session, string _vbd)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<VBD>("VBD.get_record", new JArray(session, _vbd ?? ""), serializer);
        }

        public XenRef<VBD> vbd_get_by_uuid(string session, string _uuid)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<VBD>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<VBD>>("VBD.get_by_uuid", new JArray(session, _uuid ?? ""), serializer);
        }

        public XenRef<VBD> vbd_create(string session, VBD _record)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<VBD>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<VBD>>("VBD.create", new JArray(session, _record.ToJObject()), serializer);
        }

        public XenRef<Task> async_vbd_create(string session, VBD _record)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.VBD.create", new JArray(session, _record.ToJObject()), serializer);
        }

        public void vbd_destroy(string session, string _vbd)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("VBD.destroy", new JArray(session, _vbd ?? ""), serializer);
        }

        public XenRef<Task> async_vbd_destroy(string session, string _vbd)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.VBD.destroy", new JArray(session, _vbd ?? ""), serializer);
        }

        public string vbd_get_uuid(string session, string _vbd)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("VBD.get_uuid", new JArray(session, _vbd ?? ""), serializer);
        }

        public List<vbd_operations> vbd_get_allowed_operations(string session, string _vbd)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<List<vbd_operations>>("VBD.get_allowed_operations", new JArray(session, _vbd ?? ""), serializer);
        }

        public Dictionary<string, vbd_operations> vbd_get_current_operations(string session, string _vbd)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<string, vbd_operations>>("VBD.get_current_operations", new JArray(session, _vbd ?? ""), serializer);
        }

        public XenRef<VM> vbd_get_vm(string session, string _vbd)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<VM>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<VM>>("VBD.get_VM", new JArray(session, _vbd ?? ""), serializer);
        }

        public XenRef<VDI> vbd_get_vdi(string session, string _vbd)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<VDI>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<VDI>>("VBD.get_VDI", new JArray(session, _vbd ?? ""), serializer);
        }

        public string vbd_get_device(string session, string _vbd)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("VBD.get_device", new JArray(session, _vbd ?? ""), serializer);
        }

        public string vbd_get_userdevice(string session, string _vbd)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("VBD.get_userdevice", new JArray(session, _vbd ?? ""), serializer);
        }

        public bool vbd_get_bootable(string session, string _vbd)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<bool>("VBD.get_bootable", new JArray(session, _vbd ?? ""), serializer);
        }

        public vbd_mode vbd_get_mode(string session, string _vbd)
        {
            var converters = new List<JsonConverter> {new vbd_modeConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<vbd_mode>("VBD.get_mode", new JArray(session, _vbd ?? ""), serializer);
        }

        public vbd_type vbd_get_type(string session, string _vbd)
        {
            var converters = new List<JsonConverter> {new vbd_typeConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<vbd_type>("VBD.get_type", new JArray(session, _vbd ?? ""), serializer);
        }

        public bool vbd_get_unpluggable(string session, string _vbd)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<bool>("VBD.get_unpluggable", new JArray(session, _vbd ?? ""), serializer);
        }

        public bool vbd_get_storage_lock(string session, string _vbd)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<bool>("VBD.get_storage_lock", new JArray(session, _vbd ?? ""), serializer);
        }

        public bool vbd_get_empty(string session, string _vbd)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<bool>("VBD.get_empty", new JArray(session, _vbd ?? ""), serializer);
        }

        public Dictionary<string, string> vbd_get_other_config(string session, string _vbd)
        {
            var converters = new List<JsonConverter> {new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<string, string>>("VBD.get_other_config", new JArray(session, _vbd ?? ""), serializer);
        }

        public bool vbd_get_currently_attached(string session, string _vbd)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<bool>("VBD.get_currently_attached", new JArray(session, _vbd ?? ""), serializer);
        }

        public long vbd_get_status_code(string session, string _vbd)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<long>("VBD.get_status_code", new JArray(session, _vbd ?? ""), serializer);
        }

        public string vbd_get_status_detail(string session, string _vbd)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("VBD.get_status_detail", new JArray(session, _vbd ?? ""), serializer);
        }

        public Dictionary<string, string> vbd_get_runtime_properties(string session, string _vbd)
        {
            var converters = new List<JsonConverter> {new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<string, string>>("VBD.get_runtime_properties", new JArray(session, _vbd ?? ""), serializer);
        }

        public string vbd_get_qos_algorithm_type(string session, string _vbd)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("VBD.get_qos_algorithm_type", new JArray(session, _vbd ?? ""), serializer);
        }

        public Dictionary<string, string> vbd_get_qos_algorithm_params(string session, string _vbd)
        {
            var converters = new List<JsonConverter> {new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<string, string>>("VBD.get_qos_algorithm_params", new JArray(session, _vbd ?? ""), serializer);
        }

        public string[] vbd_get_qos_supported_algorithms(string session, string _vbd)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string[]>("VBD.get_qos_supported_algorithms", new JArray(session, _vbd ?? ""), serializer);
        }

        public XenRef<VBD_metrics> vbd_get_metrics(string session, string _vbd)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<VBD_metrics>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<VBD_metrics>>("VBD.get_metrics", new JArray(session, _vbd ?? ""), serializer);
        }

        public void vbd_set_userdevice(string session, string _vbd, string _userdevice)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("VBD.set_userdevice", new JArray(session, _vbd ?? "", _userdevice ?? ""), serializer);
        }

        public void vbd_set_bootable(string session, string _vbd, bool _bootable)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("VBD.set_bootable", new JArray(session, _vbd ?? "", _bootable), serializer);
        }

        public void vbd_set_type(string session, string _vbd, vbd_type _type)
        {
            var converters = new List<JsonConverter> {new vbd_typeConverter()};
            var serializer = CreateSerializer(converters);
            Rpc("VBD.set_type", new JArray(session, _vbd ?? "", _type.StringOf()), serializer);
        }

        public void vbd_set_unpluggable(string session, string _vbd, bool _unpluggable)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("VBD.set_unpluggable", new JArray(session, _vbd ?? "", _unpluggable), serializer);
        }

        public void vbd_set_other_config(string session, string _vbd, Dictionary<string, string> _other_config)
        {
            var converters = new List<JsonConverter> {new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            Rpc("VBD.set_other_config", new JArray(session, _vbd ?? "", _other_config == null ? new JObject() : JObject.FromObject(_other_config, serializer)), serializer);
        }

        public void vbd_add_to_other_config(string session, string _vbd, string _key, string _value)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("VBD.add_to_other_config", new JArray(session, _vbd ?? "", _key ?? "", _value ?? ""), serializer);
        }

        public void vbd_remove_from_other_config(string session, string _vbd, string _key)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("VBD.remove_from_other_config", new JArray(session, _vbd ?? "", _key ?? ""), serializer);
        }

        public void vbd_set_qos_algorithm_type(string session, string _vbd, string _algorithm_type)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("VBD.set_qos_algorithm_type", new JArray(session, _vbd ?? "", _algorithm_type ?? ""), serializer);
        }

        public void vbd_set_qos_algorithm_params(string session, string _vbd, Dictionary<string, string> _algorithm_params)
        {
            var converters = new List<JsonConverter> {new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            Rpc("VBD.set_qos_algorithm_params", new JArray(session, _vbd ?? "", _algorithm_params == null ? new JObject() : JObject.FromObject(_algorithm_params, serializer)), serializer);
        }

        public void vbd_add_to_qos_algorithm_params(string session, string _vbd, string _key, string _value)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("VBD.add_to_qos_algorithm_params", new JArray(session, _vbd ?? "", _key ?? "", _value ?? ""), serializer);
        }

        public void vbd_remove_from_qos_algorithm_params(string session, string _vbd, string _key)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("VBD.remove_from_qos_algorithm_params", new JArray(session, _vbd ?? "", _key ?? ""), serializer);
        }

        public void vbd_eject(string session, string _vbd)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("VBD.eject", new JArray(session, _vbd ?? ""), serializer);
        }

        public XenRef<Task> async_vbd_eject(string session, string _vbd)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.VBD.eject", new JArray(session, _vbd ?? ""), serializer);
        }

        public void vbd_insert(string session, string _vbd, string _vdi)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<VDI>()};
            var serializer = CreateSerializer(converters);
            Rpc("VBD.insert", new JArray(session, _vbd ?? "", _vdi ?? ""), serializer);
        }

        public XenRef<Task> async_vbd_insert(string session, string _vbd, string _vdi)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>(), new XenRefConverter<VDI>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.VBD.insert", new JArray(session, _vbd ?? "", _vdi ?? ""), serializer);
        }

        public void vbd_plug(string session, string _vbd)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("VBD.plug", new JArray(session, _vbd ?? ""), serializer);
        }

        public XenRef<Task> async_vbd_plug(string session, string _vbd)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.VBD.plug", new JArray(session, _vbd ?? ""), serializer);
        }

        public void vbd_unplug(string session, string _vbd)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("VBD.unplug", new JArray(session, _vbd ?? ""), serializer);
        }

        public XenRef<Task> async_vbd_unplug(string session, string _vbd)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.VBD.unplug", new JArray(session, _vbd ?? ""), serializer);
        }

        public void vbd_unplug_force(string session, string _vbd)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("VBD.unplug_force", new JArray(session, _vbd ?? ""), serializer);
        }

        public XenRef<Task> async_vbd_unplug_force(string session, string _vbd)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.VBD.unplug_force", new JArray(session, _vbd ?? ""), serializer);
        }

        public void vbd_assert_attachable(string session, string _vbd)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("VBD.assert_attachable", new JArray(session, _vbd ?? ""), serializer);
        }

        public XenRef<Task> async_vbd_assert_attachable(string session, string _vbd)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.VBD.assert_attachable", new JArray(session, _vbd ?? ""), serializer);
        }

        public void vbd_set_mode(string session, string _vbd, vbd_mode _value)
        {
            var converters = new List<JsonConverter> {new vbd_modeConverter()};
            var serializer = CreateSerializer(converters);
            Rpc("VBD.set_mode", new JArray(session, _vbd ?? "", _value.StringOf()), serializer);
        }

        public XenRef<Task> async_vbd_set_mode(string session, string _vbd, vbd_mode _value)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>(), new vbd_modeConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.VBD.set_mode", new JArray(session, _vbd ?? "", _value.StringOf()), serializer);
        }

        public List<XenRef<VBD>> vbd_get_all(string session)
        {
            var converters = new List<JsonConverter> {new XenRefListConverter<VBD>()};
            var serializer = CreateSerializer(converters);
            return Rpc<List<XenRef<VBD>>>("VBD.get_all", new JArray(session), serializer);
        }

        public Dictionary<XenRef<VBD>, VBD> vbd_get_all_records(string session)
        {
            var converters = new List<JsonConverter> {new XenRefXenObjectMapConverter<VBD>()};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<XenRef<VBD>, VBD>>("VBD.get_all_records", new JArray(session), serializer);
        }

        public VBD_metrics vbd_metrics_get_record(string session, string _vbd_metrics)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<VBD_metrics>("VBD_metrics.get_record", new JArray(session, _vbd_metrics ?? ""), serializer);
        }

        public XenRef<VBD_metrics> vbd_metrics_get_by_uuid(string session, string _uuid)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<VBD_metrics>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<VBD_metrics>>("VBD_metrics.get_by_uuid", new JArray(session, _uuid ?? ""), serializer);
        }

        public string vbd_metrics_get_uuid(string session, string _vbd_metrics)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("VBD_metrics.get_uuid", new JArray(session, _vbd_metrics ?? ""), serializer);
        }

        public double vbd_metrics_get_io_read_kbs(string session, string _vbd_metrics)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<double>("VBD_metrics.get_io_read_kbs", new JArray(session, _vbd_metrics ?? ""), serializer);
        }

        public double vbd_metrics_get_io_write_kbs(string session, string _vbd_metrics)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<double>("VBD_metrics.get_io_write_kbs", new JArray(session, _vbd_metrics ?? ""), serializer);
        }

        public DateTime vbd_metrics_get_last_updated(string session, string _vbd_metrics)
        {
            var converters = new List<JsonConverter> {new XenDateTimeConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<DateTime>("VBD_metrics.get_last_updated", new JArray(session, _vbd_metrics ?? ""), serializer);
        }

        public Dictionary<string, string> vbd_metrics_get_other_config(string session, string _vbd_metrics)
        {
            var converters = new List<JsonConverter> {new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<string, string>>("VBD_metrics.get_other_config", new JArray(session, _vbd_metrics ?? ""), serializer);
        }

        public void vbd_metrics_set_other_config(string session, string _vbd_metrics, Dictionary<string, string> _other_config)
        {
            var converters = new List<JsonConverter> {new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            Rpc("VBD_metrics.set_other_config", new JArray(session, _vbd_metrics ?? "", _other_config == null ? new JObject() : JObject.FromObject(_other_config, serializer)), serializer);
        }

        public void vbd_metrics_add_to_other_config(string session, string _vbd_metrics, string _key, string _value)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("VBD_metrics.add_to_other_config", new JArray(session, _vbd_metrics ?? "", _key ?? "", _value ?? ""), serializer);
        }

        public void vbd_metrics_remove_from_other_config(string session, string _vbd_metrics, string _key)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("VBD_metrics.remove_from_other_config", new JArray(session, _vbd_metrics ?? "", _key ?? ""), serializer);
        }

        public List<XenRef<VBD_metrics>> vbd_metrics_get_all(string session)
        {
            var converters = new List<JsonConverter> {new XenRefListConverter<VBD_metrics>()};
            var serializer = CreateSerializer(converters);
            return Rpc<List<XenRef<VBD_metrics>>>("VBD_metrics.get_all", new JArray(session), serializer);
        }

        public Dictionary<XenRef<VBD_metrics>, VBD_metrics> vbd_metrics_get_all_records(string session)
        {
            var converters = new List<JsonConverter> {new XenRefXenObjectMapConverter<VBD_metrics>()};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<XenRef<VBD_metrics>, VBD_metrics>>("VBD_metrics.get_all_records", new JArray(session), serializer);
        }

        public PBD pbd_get_record(string session, string _pbd)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<PBD>("PBD.get_record", new JArray(session, _pbd ?? ""), serializer);
        }

        public XenRef<PBD> pbd_get_by_uuid(string session, string _uuid)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<PBD>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<PBD>>("PBD.get_by_uuid", new JArray(session, _uuid ?? ""), serializer);
        }

        public XenRef<PBD> pbd_create(string session, PBD _record)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<PBD>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<PBD>>("PBD.create", new JArray(session, _record.ToJObject()), serializer);
        }

        public XenRef<Task> async_pbd_create(string session, PBD _record)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.PBD.create", new JArray(session, _record.ToJObject()), serializer);
        }

        public void pbd_destroy(string session, string _pbd)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("PBD.destroy", new JArray(session, _pbd ?? ""), serializer);
        }

        public XenRef<Task> async_pbd_destroy(string session, string _pbd)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.PBD.destroy", new JArray(session, _pbd ?? ""), serializer);
        }

        public string pbd_get_uuid(string session, string _pbd)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("PBD.get_uuid", new JArray(session, _pbd ?? ""), serializer);
        }

        public XenRef<Host> pbd_get_host(string session, string _pbd)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Host>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Host>>("PBD.get_host", new JArray(session, _pbd ?? ""), serializer);
        }

        public XenRef<SR> pbd_get_sr(string session, string _pbd)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<SR>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<SR>>("PBD.get_SR", new JArray(session, _pbd ?? ""), serializer);
        }

        public Dictionary<string, string> pbd_get_device_config(string session, string _pbd)
        {
            var converters = new List<JsonConverter> {new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<string, string>>("PBD.get_device_config", new JArray(session, _pbd ?? ""), serializer);
        }

        public bool pbd_get_currently_attached(string session, string _pbd)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<bool>("PBD.get_currently_attached", new JArray(session, _pbd ?? ""), serializer);
        }

        public Dictionary<string, string> pbd_get_other_config(string session, string _pbd)
        {
            var converters = new List<JsonConverter> {new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<string, string>>("PBD.get_other_config", new JArray(session, _pbd ?? ""), serializer);
        }

        public void pbd_set_other_config(string session, string _pbd, Dictionary<string, string> _other_config)
        {
            var converters = new List<JsonConverter> {new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            Rpc("PBD.set_other_config", new JArray(session, _pbd ?? "", _other_config == null ? new JObject() : JObject.FromObject(_other_config, serializer)), serializer);
        }

        public void pbd_add_to_other_config(string session, string _pbd, string _key, string _value)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("PBD.add_to_other_config", new JArray(session, _pbd ?? "", _key ?? "", _value ?? ""), serializer);
        }

        public void pbd_remove_from_other_config(string session, string _pbd, string _key)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("PBD.remove_from_other_config", new JArray(session, _pbd ?? "", _key ?? ""), serializer);
        }

        public void pbd_plug(string session, string _pbd)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("PBD.plug", new JArray(session, _pbd ?? ""), serializer);
        }

        public XenRef<Task> async_pbd_plug(string session, string _pbd)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.PBD.plug", new JArray(session, _pbd ?? ""), serializer);
        }

        public void pbd_unplug(string session, string _pbd)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("PBD.unplug", new JArray(session, _pbd ?? ""), serializer);
        }

        public XenRef<Task> async_pbd_unplug(string session, string _pbd)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.PBD.unplug", new JArray(session, _pbd ?? ""), serializer);
        }

        public void pbd_set_device_config(string session, string _pbd, Dictionary<string, string> _value)
        {
            var converters = new List<JsonConverter> {new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            Rpc("PBD.set_device_config", new JArray(session, _pbd ?? "", _value == null ? new JObject() : JObject.FromObject(_value, serializer)), serializer);
        }

        public XenRef<Task> async_pbd_set_device_config(string session, string _pbd, Dictionary<string, string> _value)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>(), new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.PBD.set_device_config", new JArray(session, _pbd ?? "", _value == null ? new JObject() : JObject.FromObject(_value, serializer)), serializer);
        }

        public List<XenRef<PBD>> pbd_get_all(string session)
        {
            var converters = new List<JsonConverter> {new XenRefListConverter<PBD>()};
            var serializer = CreateSerializer(converters);
            return Rpc<List<XenRef<PBD>>>("PBD.get_all", new JArray(session), serializer);
        }

        public Dictionary<XenRef<PBD>, PBD> pbd_get_all_records(string session)
        {
            var converters = new List<JsonConverter> {new XenRefXenObjectMapConverter<PBD>()};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<XenRef<PBD>, PBD>>("PBD.get_all_records", new JArray(session), serializer);
        }

        public Crashdump crashdump_get_record(string session, string _crashdump)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<Crashdump>("crashdump.get_record", new JArray(session, _crashdump ?? ""), serializer);
        }

        public XenRef<Crashdump> crashdump_get_by_uuid(string session, string _uuid)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Crashdump>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Crashdump>>("crashdump.get_by_uuid", new JArray(session, _uuid ?? ""), serializer);
        }

        public string crashdump_get_uuid(string session, string _crashdump)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("crashdump.get_uuid", new JArray(session, _crashdump ?? ""), serializer);
        }

        public XenRef<VM> crashdump_get_vm(string session, string _crashdump)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<VM>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<VM>>("crashdump.get_VM", new JArray(session, _crashdump ?? ""), serializer);
        }

        public XenRef<VDI> crashdump_get_vdi(string session, string _crashdump)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<VDI>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<VDI>>("crashdump.get_VDI", new JArray(session, _crashdump ?? ""), serializer);
        }

        public Dictionary<string, string> crashdump_get_other_config(string session, string _crashdump)
        {
            var converters = new List<JsonConverter> {new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<string, string>>("crashdump.get_other_config", new JArray(session, _crashdump ?? ""), serializer);
        }

        public void crashdump_set_other_config(string session, string _crashdump, Dictionary<string, string> _other_config)
        {
            var converters = new List<JsonConverter> {new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            Rpc("crashdump.set_other_config", new JArray(session, _crashdump ?? "", _other_config == null ? new JObject() : JObject.FromObject(_other_config, serializer)), serializer);
        }

        public void crashdump_add_to_other_config(string session, string _crashdump, string _key, string _value)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("crashdump.add_to_other_config", new JArray(session, _crashdump ?? "", _key ?? "", _value ?? ""), serializer);
        }

        public void crashdump_remove_from_other_config(string session, string _crashdump, string _key)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("crashdump.remove_from_other_config", new JArray(session, _crashdump ?? "", _key ?? ""), serializer);
        }

        public void crashdump_destroy(string session, string _crashdump)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("crashdump.destroy", new JArray(session, _crashdump ?? ""), serializer);
        }

        public XenRef<Task> async_crashdump_destroy(string session, string _crashdump)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.crashdump.destroy", new JArray(session, _crashdump ?? ""), serializer);
        }

        public List<XenRef<Crashdump>> crashdump_get_all(string session)
        {
            var converters = new List<JsonConverter> {new XenRefListConverter<Crashdump>()};
            var serializer = CreateSerializer(converters);
            return Rpc<List<XenRef<Crashdump>>>("crashdump.get_all", new JArray(session), serializer);
        }

        public Dictionary<XenRef<Crashdump>, Crashdump> crashdump_get_all_records(string session)
        {
            var converters = new List<JsonConverter> {new XenRefXenObjectMapConverter<Crashdump>()};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<XenRef<Crashdump>, Crashdump>>("crashdump.get_all_records", new JArray(session), serializer);
        }

        public VTPM vtpm_get_record(string session, string _vtpm)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<VTPM>("VTPM.get_record", new JArray(session, _vtpm ?? ""), serializer);
        }

        public XenRef<VTPM> vtpm_get_by_uuid(string session, string _uuid)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<VTPM>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<VTPM>>("VTPM.get_by_uuid", new JArray(session, _uuid ?? ""), serializer);
        }

        public XenRef<VTPM> vtpm_create(string session, VTPM _record)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<VTPM>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<VTPM>>("VTPM.create", new JArray(session, _record.ToJObject()), serializer);
        }

        public XenRef<Task> async_vtpm_create(string session, VTPM _record)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.VTPM.create", new JArray(session, _record.ToJObject()), serializer);
        }

        public void vtpm_destroy(string session, string _vtpm)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("VTPM.destroy", new JArray(session, _vtpm ?? ""), serializer);
        }

        public XenRef<Task> async_vtpm_destroy(string session, string _vtpm)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.VTPM.destroy", new JArray(session, _vtpm ?? ""), serializer);
        }

        public string vtpm_get_uuid(string session, string _vtpm)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("VTPM.get_uuid", new JArray(session, _vtpm ?? ""), serializer);
        }

        public XenRef<VM> vtpm_get_vm(string session, string _vtpm)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<VM>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<VM>>("VTPM.get_VM", new JArray(session, _vtpm ?? ""), serializer);
        }

        public XenRef<VM> vtpm_get_backend(string session, string _vtpm)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<VM>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<VM>>("VTPM.get_backend", new JArray(session, _vtpm ?? ""), serializer);
        }

        public Dictionary<XenRef<VTPM>, VTPM> vtpm_get_all_records(string session)
        {
            var converters = new List<JsonConverter> {new XenRefXenObjectMapConverter<VTPM>()};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<XenRef<VTPM>, VTPM>>("VTPM.get_all_records", new JArray(session), serializer);
        }

        public Console console_get_record(string session, string _console)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<Console>("console.get_record", new JArray(session, _console ?? ""), serializer);
        }

        public XenRef<Console> console_get_by_uuid(string session, string _uuid)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Console>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Console>>("console.get_by_uuid", new JArray(session, _uuid ?? ""), serializer);
        }

        public XenRef<Console> console_create(string session, Console _record)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Console>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Console>>("console.create", new JArray(session, _record.ToJObject()), serializer);
        }

        public XenRef<Task> async_console_create(string session, Console _record)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.console.create", new JArray(session, _record.ToJObject()), serializer);
        }

        public void console_destroy(string session, string _console)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("console.destroy", new JArray(session, _console ?? ""), serializer);
        }

        public XenRef<Task> async_console_destroy(string session, string _console)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.console.destroy", new JArray(session, _console ?? ""), serializer);
        }

        public string console_get_uuid(string session, string _console)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("console.get_uuid", new JArray(session, _console ?? ""), serializer);
        }

        public console_protocol console_get_protocol(string session, string _console)
        {
            var converters = new List<JsonConverter> {new console_protocolConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<console_protocol>("console.get_protocol", new JArray(session, _console ?? ""), serializer);
        }

        public string console_get_location(string session, string _console)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("console.get_location", new JArray(session, _console ?? ""), serializer);
        }

        public XenRef<VM> console_get_vm(string session, string _console)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<VM>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<VM>>("console.get_VM", new JArray(session, _console ?? ""), serializer);
        }

        public Dictionary<string, string> console_get_other_config(string session, string _console)
        {
            var converters = new List<JsonConverter> {new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<string, string>>("console.get_other_config", new JArray(session, _console ?? ""), serializer);
        }

        public void console_set_other_config(string session, string _console, Dictionary<string, string> _other_config)
        {
            var converters = new List<JsonConverter> {new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            Rpc("console.set_other_config", new JArray(session, _console ?? "", _other_config == null ? new JObject() : JObject.FromObject(_other_config, serializer)), serializer);
        }

        public void console_add_to_other_config(string session, string _console, string _key, string _value)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("console.add_to_other_config", new JArray(session, _console ?? "", _key ?? "", _value ?? ""), serializer);
        }

        public void console_remove_from_other_config(string session, string _console, string _key)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("console.remove_from_other_config", new JArray(session, _console ?? "", _key ?? ""), serializer);
        }

        public List<XenRef<Console>> console_get_all(string session)
        {
            var converters = new List<JsonConverter> {new XenRefListConverter<Console>()};
            var serializer = CreateSerializer(converters);
            return Rpc<List<XenRef<Console>>>("console.get_all", new JArray(session), serializer);
        }

        public Dictionary<XenRef<Console>, Console> console_get_all_records(string session)
        {
            var converters = new List<JsonConverter> {new XenRefXenObjectMapConverter<Console>()};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<XenRef<Console>, Console>>("console.get_all_records", new JArray(session), serializer);
        }

        public User user_get_record(string session, string _user)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<User>("user.get_record", new JArray(session, _user ?? ""), serializer);
        }

        public XenRef<User> user_get_by_uuid(string session, string _uuid)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<User>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<User>>("user.get_by_uuid", new JArray(session, _uuid ?? ""), serializer);
        }

        public XenRef<User> user_create(string session, User _record)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<User>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<User>>("user.create", new JArray(session, _record.ToJObject()), serializer);
        }

        public XenRef<Task> async_user_create(string session, User _record)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.user.create", new JArray(session, _record.ToJObject()), serializer);
        }

        public void user_destroy(string session, string _user)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("user.destroy", new JArray(session, _user ?? ""), serializer);
        }

        public XenRef<Task> async_user_destroy(string session, string _user)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.user.destroy", new JArray(session, _user ?? ""), serializer);
        }

        public string user_get_uuid(string session, string _user)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("user.get_uuid", new JArray(session, _user ?? ""), serializer);
        }

        public string user_get_short_name(string session, string _user)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("user.get_short_name", new JArray(session, _user ?? ""), serializer);
        }

        public string user_get_fullname(string session, string _user)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("user.get_fullname", new JArray(session, _user ?? ""), serializer);
        }

        public Dictionary<string, string> user_get_other_config(string session, string _user)
        {
            var converters = new List<JsonConverter> {new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<string, string>>("user.get_other_config", new JArray(session, _user ?? ""), serializer);
        }

        public void user_set_fullname(string session, string _user, string _fullname)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("user.set_fullname", new JArray(session, _user ?? "", _fullname ?? ""), serializer);
        }

        public void user_set_other_config(string session, string _user, Dictionary<string, string> _other_config)
        {
            var converters = new List<JsonConverter> {new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            Rpc("user.set_other_config", new JArray(session, _user ?? "", _other_config == null ? new JObject() : JObject.FromObject(_other_config, serializer)), serializer);
        }

        public void user_add_to_other_config(string session, string _user, string _key, string _value)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("user.add_to_other_config", new JArray(session, _user ?? "", _key ?? "", _value ?? ""), serializer);
        }

        public void user_remove_from_other_config(string session, string _user, string _key)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("user.remove_from_other_config", new JArray(session, _user ?? "", _key ?? ""), serializer);
        }

        public Dictionary<XenRef<User>, User> user_get_all_records(string session)
        {
            var converters = new List<JsonConverter> {new XenRefXenObjectMapConverter<User>()};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<XenRef<User>, User>>("user.get_all_records", new JArray(session), serializer);
        }

        public Dictionary<XenRef<Data_source>, Data_source> data_source_get_all_records(string session)
        {
            var converters = new List<JsonConverter> {new XenRefXenObjectMapConverter<Data_source>()};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<XenRef<Data_source>, Data_source>>("data_source.get_all_records", new JArray(session), serializer);
        }

        public Blob blob_get_record(string session, string _blob)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<Blob>("blob.get_record", new JArray(session, _blob ?? ""), serializer);
        }

        public XenRef<Blob> blob_get_by_uuid(string session, string _uuid)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Blob>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Blob>>("blob.get_by_uuid", new JArray(session, _uuid ?? ""), serializer);
        }

        public List<XenRef<Blob>> blob_get_by_name_label(string session, string _label)
        {
            var converters = new List<JsonConverter> {new XenRefListConverter<Blob>()};
            var serializer = CreateSerializer(converters);
            return Rpc<List<XenRef<Blob>>>("blob.get_by_name_label", new JArray(session, _label ?? ""), serializer);
        }

        public string blob_get_uuid(string session, string _blob)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("blob.get_uuid", new JArray(session, _blob ?? ""), serializer);
        }

        public string blob_get_name_label(string session, string _blob)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("blob.get_name_label", new JArray(session, _blob ?? ""), serializer);
        }

        public string blob_get_name_description(string session, string _blob)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("blob.get_name_description", new JArray(session, _blob ?? ""), serializer);
        }

        public long blob_get_size(string session, string _blob)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<long>("blob.get_size", new JArray(session, _blob ?? ""), serializer);
        }

        public bool blob_get_public(string session, string _blob)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<bool>("blob.get_public", new JArray(session, _blob ?? ""), serializer);
        }

        public DateTime blob_get_last_updated(string session, string _blob)
        {
            var converters = new List<JsonConverter> {new XenDateTimeConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<DateTime>("blob.get_last_updated", new JArray(session, _blob ?? ""), serializer);
        }

        public string blob_get_mime_type(string session, string _blob)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("blob.get_mime_type", new JArray(session, _blob ?? ""), serializer);
        }

        public void blob_set_name_label(string session, string _blob, string _label)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("blob.set_name_label", new JArray(session, _blob ?? "", _label ?? ""), serializer);
        }

        public void blob_set_name_description(string session, string _blob, string _description)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("blob.set_name_description", new JArray(session, _blob ?? "", _description ?? ""), serializer);
        }

        public void blob_set_public(string session, string _blob, bool _public)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("blob.set_public", new JArray(session, _blob ?? "", _public), serializer);
        }

        public XenRef<Blob> blob_create(string session, string _mime_type)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Blob>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Blob>>("blob.create", new JArray(session, _mime_type ?? ""), serializer);
        }

        public XenRef<Blob> blob_create(string session, string _mime_type, bool _public)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Blob>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Blob>>("blob.create", new JArray(session, _mime_type ?? "", _public), serializer);
        }

        public void blob_destroy(string session, string _blob)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("blob.destroy", new JArray(session, _blob ?? ""), serializer);
        }

        public List<XenRef<Blob>> blob_get_all(string session)
        {
            var converters = new List<JsonConverter> {new XenRefListConverter<Blob>()};
            var serializer = CreateSerializer(converters);
            return Rpc<List<XenRef<Blob>>>("blob.get_all", new JArray(session), serializer);
        }

        public Dictionary<XenRef<Blob>, Blob> blob_get_all_records(string session)
        {
            var converters = new List<JsonConverter> {new XenRefXenObjectMapConverter<Blob>()};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<XenRef<Blob>, Blob>>("blob.get_all_records", new JArray(session), serializer);
        }

        public XenRef<Message> message_create(string session, string _name, long _priority, cls _cls, string _obj_uuid, string _body)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Message>(), new clsConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Message>>("message.create", new JArray(session, _name ?? "", _priority, _cls.StringOf(), _obj_uuid ?? "", _body ?? ""), serializer);
        }

        public void message_destroy(string session, string _message)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("message.destroy", new JArray(session, _message ?? ""), serializer);
        }

        public Dictionary<XenRef<Message>, Message> message_get(string session, cls _cls, string _obj_uuid, DateTime _since)
        {
            var converters = new List<JsonConverter> {new XenRefXenObjectMapConverter<Message>(), new clsConverter(), new XenDateTimeConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<XenRef<Message>, Message>>("message.get", new JArray(session, _cls.StringOf(), _obj_uuid ?? "", _since), serializer);
        }

        public List<XenRef<Message>> message_get_all(string session)
        {
            var converters = new List<JsonConverter> {new XenRefListConverter<Message>()};
            var serializer = CreateSerializer(converters);
            return Rpc<List<XenRef<Message>>>("message.get_all", new JArray(session), serializer);
        }

        public Dictionary<XenRef<Message>, Message> message_get_since(string session, DateTime _since)
        {
            var converters = new List<JsonConverter> {new XenRefXenObjectMapConverter<Message>(), new XenDateTimeConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<XenRef<Message>, Message>>("message.get_since", new JArray(session, _since), serializer);
        }

        public Message message_get_record(string session, string _message)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<Message>("message.get_record", new JArray(session, _message ?? ""), serializer);
        }

        public XenRef<Message> message_get_by_uuid(string session, string _uuid)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Message>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Message>>("message.get_by_uuid", new JArray(session, _uuid ?? ""), serializer);
        }

        public Dictionary<XenRef<Message>, Message> message_get_all_records(string session)
        {
            var converters = new List<JsonConverter> {new XenRefXenObjectMapConverter<Message>()};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<XenRef<Message>, Message>>("message.get_all_records", new JArray(session), serializer);
        }

        public Dictionary<XenRef<Message>, Message> message_get_all_records_where(string session, string _expr)
        {
            var converters = new List<JsonConverter> {new XenRefXenObjectMapConverter<Message>()};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<XenRef<Message>, Message>>("message.get_all_records_where", new JArray(session, _expr ?? ""), serializer);
        }

        public Secret secret_get_record(string session, string _secret)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<Secret>("secret.get_record", new JArray(session, _secret ?? ""), serializer);
        }

        public XenRef<Secret> secret_get_by_uuid(string session, string _uuid)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Secret>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Secret>>("secret.get_by_uuid", new JArray(session, _uuid ?? ""), serializer);
        }

        public XenRef<Secret> secret_create(string session, Secret _record)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Secret>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Secret>>("secret.create", new JArray(session, _record.ToJObject()), serializer);
        }

        public XenRef<Task> async_secret_create(string session, Secret _record)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.secret.create", new JArray(session, _record.ToJObject()), serializer);
        }

        public void secret_destroy(string session, string _secret)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("secret.destroy", new JArray(session, _secret ?? ""), serializer);
        }

        public XenRef<Task> async_secret_destroy(string session, string _secret)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.secret.destroy", new JArray(session, _secret ?? ""), serializer);
        }

        public string secret_get_uuid(string session, string _secret)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("secret.get_uuid", new JArray(session, _secret ?? ""), serializer);
        }

        public string secret_get_value(string session, string _secret)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("secret.get_value", new JArray(session, _secret ?? ""), serializer);
        }

        public Dictionary<string, string> secret_get_other_config(string session, string _secret)
        {
            var converters = new List<JsonConverter> {new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<string, string>>("secret.get_other_config", new JArray(session, _secret ?? ""), serializer);
        }

        public void secret_set_value(string session, string _secret, string _value)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("secret.set_value", new JArray(session, _secret ?? "", _value ?? ""), serializer);
        }

        public void secret_set_other_config(string session, string _secret, Dictionary<string, string> _other_config)
        {
            var converters = new List<JsonConverter> {new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            Rpc("secret.set_other_config", new JArray(session, _secret ?? "", _other_config == null ? new JObject() : JObject.FromObject(_other_config, serializer)), serializer);
        }

        public void secret_add_to_other_config(string session, string _secret, string _key, string _value)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("secret.add_to_other_config", new JArray(session, _secret ?? "", _key ?? "", _value ?? ""), serializer);
        }

        public void secret_remove_from_other_config(string session, string _secret, string _key)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("secret.remove_from_other_config", new JArray(session, _secret ?? "", _key ?? ""), serializer);
        }

        public List<XenRef<Secret>> secret_get_all(string session)
        {
            var converters = new List<JsonConverter> {new XenRefListConverter<Secret>()};
            var serializer = CreateSerializer(converters);
            return Rpc<List<XenRef<Secret>>>("secret.get_all", new JArray(session), serializer);
        }

        public Dictionary<XenRef<Secret>, Secret> secret_get_all_records(string session)
        {
            var converters = new List<JsonConverter> {new XenRefXenObjectMapConverter<Secret>()};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<XenRef<Secret>, Secret>>("secret.get_all_records", new JArray(session), serializer);
        }

        public Tunnel tunnel_get_record(string session, string _tunnel)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<Tunnel>("tunnel.get_record", new JArray(session, _tunnel ?? ""), serializer);
        }

        public XenRef<Tunnel> tunnel_get_by_uuid(string session, string _uuid)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Tunnel>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Tunnel>>("tunnel.get_by_uuid", new JArray(session, _uuid ?? ""), serializer);
        }

        public string tunnel_get_uuid(string session, string _tunnel)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("tunnel.get_uuid", new JArray(session, _tunnel ?? ""), serializer);
        }

        public XenRef<PIF> tunnel_get_access_pif(string session, string _tunnel)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<PIF>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<PIF>>("tunnel.get_access_PIF", new JArray(session, _tunnel ?? ""), serializer);
        }

        public XenRef<PIF> tunnel_get_transport_pif(string session, string _tunnel)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<PIF>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<PIF>>("tunnel.get_transport_PIF", new JArray(session, _tunnel ?? ""), serializer);
        }

        public Dictionary<string, string> tunnel_get_status(string session, string _tunnel)
        {
            var converters = new List<JsonConverter> {new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<string, string>>("tunnel.get_status", new JArray(session, _tunnel ?? ""), serializer);
        }

        public Dictionary<string, string> tunnel_get_other_config(string session, string _tunnel)
        {
            var converters = new List<JsonConverter> {new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<string, string>>("tunnel.get_other_config", new JArray(session, _tunnel ?? ""), serializer);
        }

        public void tunnel_set_status(string session, string _tunnel, Dictionary<string, string> _status)
        {
            var converters = new List<JsonConverter> {new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            Rpc("tunnel.set_status", new JArray(session, _tunnel ?? "", _status == null ? new JObject() : JObject.FromObject(_status, serializer)), serializer);
        }

        public void tunnel_add_to_status(string session, string _tunnel, string _key, string _value)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("tunnel.add_to_status", new JArray(session, _tunnel ?? "", _key ?? "", _value ?? ""), serializer);
        }

        public void tunnel_remove_from_status(string session, string _tunnel, string _key)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("tunnel.remove_from_status", new JArray(session, _tunnel ?? "", _key ?? ""), serializer);
        }

        public void tunnel_set_other_config(string session, string _tunnel, Dictionary<string, string> _other_config)
        {
            var converters = new List<JsonConverter> {new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            Rpc("tunnel.set_other_config", new JArray(session, _tunnel ?? "", _other_config == null ? new JObject() : JObject.FromObject(_other_config, serializer)), serializer);
        }

        public void tunnel_add_to_other_config(string session, string _tunnel, string _key, string _value)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("tunnel.add_to_other_config", new JArray(session, _tunnel ?? "", _key ?? "", _value ?? ""), serializer);
        }

        public void tunnel_remove_from_other_config(string session, string _tunnel, string _key)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("tunnel.remove_from_other_config", new JArray(session, _tunnel ?? "", _key ?? ""), serializer);
        }

        public XenRef<Tunnel> tunnel_create(string session, string _transport_pif, string _network)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Tunnel>(), new XenRefConverter<PIF>(), new XenRefConverter<Network>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Tunnel>>("tunnel.create", new JArray(session, _transport_pif ?? "", _network ?? ""), serializer);
        }

        public XenRef<Task> async_tunnel_create(string session, string _transport_pif, string _network)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>(), new XenRefConverter<PIF>(), new XenRefConverter<Network>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.tunnel.create", new JArray(session, _transport_pif ?? "", _network ?? ""), serializer);
        }

        public void tunnel_destroy(string session, string _tunnel)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("tunnel.destroy", new JArray(session, _tunnel ?? ""), serializer);
        }

        public XenRef<Task> async_tunnel_destroy(string session, string _tunnel)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.tunnel.destroy", new JArray(session, _tunnel ?? ""), serializer);
        }

        public List<XenRef<Tunnel>> tunnel_get_all(string session)
        {
            var converters = new List<JsonConverter> {new XenRefListConverter<Tunnel>()};
            var serializer = CreateSerializer(converters);
            return Rpc<List<XenRef<Tunnel>>>("tunnel.get_all", new JArray(session), serializer);
        }

        public Dictionary<XenRef<Tunnel>, Tunnel> tunnel_get_all_records(string session)
        {
            var converters = new List<JsonConverter> {new XenRefXenObjectMapConverter<Tunnel>()};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<XenRef<Tunnel>, Tunnel>>("tunnel.get_all_records", new JArray(session), serializer);
        }

        public Network_sriov network_sriov_get_record(string session, string _network_sriov)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<Network_sriov>("network_sriov.get_record", new JArray(session, _network_sriov ?? ""), serializer);
        }

        public XenRef<Network_sriov> network_sriov_get_by_uuid(string session, string _uuid)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Network_sriov>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Network_sriov>>("network_sriov.get_by_uuid", new JArray(session, _uuid ?? ""), serializer);
        }

        public string network_sriov_get_uuid(string session, string _network_sriov)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("network_sriov.get_uuid", new JArray(session, _network_sriov ?? ""), serializer);
        }

        public XenRef<PIF> network_sriov_get_physical_pif(string session, string _network_sriov)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<PIF>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<PIF>>("network_sriov.get_physical_PIF", new JArray(session, _network_sriov ?? ""), serializer);
        }

        public XenRef<PIF> network_sriov_get_logical_pif(string session, string _network_sriov)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<PIF>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<PIF>>("network_sriov.get_logical_PIF", new JArray(session, _network_sriov ?? ""), serializer);
        }

        public bool network_sriov_get_requires_reboot(string session, string _network_sriov)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<bool>("network_sriov.get_requires_reboot", new JArray(session, _network_sriov ?? ""), serializer);
        }

        public sriov_configuration_mode network_sriov_get_configuration_mode(string session, string _network_sriov)
        {
            var converters = new List<JsonConverter> {new sriov_configuration_modeConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<sriov_configuration_mode>("network_sriov.get_configuration_mode", new JArray(session, _network_sriov ?? ""), serializer);
        }

        public XenRef<Network_sriov> network_sriov_create(string session, string _pif, string _network)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Network_sriov>(), new XenRefConverter<PIF>(), new XenRefConverter<Network>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Network_sriov>>("network_sriov.create", new JArray(session, _pif ?? "", _network ?? ""), serializer);
        }

        public XenRef<Task> async_network_sriov_create(string session, string _pif, string _network)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>(), new XenRefConverter<PIF>(), new XenRefConverter<Network>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.network_sriov.create", new JArray(session, _pif ?? "", _network ?? ""), serializer);
        }

        public void network_sriov_destroy(string session, string _network_sriov)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("network_sriov.destroy", new JArray(session, _network_sriov ?? ""), serializer);
        }

        public XenRef<Task> async_network_sriov_destroy(string session, string _network_sriov)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.network_sriov.destroy", new JArray(session, _network_sriov ?? ""), serializer);
        }

        public long network_sriov_get_remaining_capacity(string session, string _network_sriov)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<long>("network_sriov.get_remaining_capacity", new JArray(session, _network_sriov ?? ""), serializer);
        }

        public XenRef<Task> async_network_sriov_get_remaining_capacity(string session, string _network_sriov)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.network_sriov.get_remaining_capacity", new JArray(session, _network_sriov ?? ""), serializer);
        }

        public List<XenRef<Network_sriov>> network_sriov_get_all(string session)
        {
            var converters = new List<JsonConverter> {new XenRefListConverter<Network_sriov>()};
            var serializer = CreateSerializer(converters);
            return Rpc<List<XenRef<Network_sriov>>>("network_sriov.get_all", new JArray(session), serializer);
        }

        public Dictionary<XenRef<Network_sriov>, Network_sriov> network_sriov_get_all_records(string session)
        {
            var converters = new List<JsonConverter> {new XenRefXenObjectMapConverter<Network_sriov>()};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<XenRef<Network_sriov>, Network_sriov>>("network_sriov.get_all_records", new JArray(session), serializer);
        }

        public PCI pci_get_record(string session, string _pci)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<PCI>("PCI.get_record", new JArray(session, _pci ?? ""), serializer);
        }

        public XenRef<PCI> pci_get_by_uuid(string session, string _uuid)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<PCI>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<PCI>>("PCI.get_by_uuid", new JArray(session, _uuid ?? ""), serializer);
        }

        public string pci_get_uuid(string session, string _pci)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("PCI.get_uuid", new JArray(session, _pci ?? ""), serializer);
        }

        public string pci_get_class_name(string session, string _pci)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("PCI.get_class_name", new JArray(session, _pci ?? ""), serializer);
        }

        public string pci_get_vendor_name(string session, string _pci)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("PCI.get_vendor_name", new JArray(session, _pci ?? ""), serializer);
        }

        public string pci_get_device_name(string session, string _pci)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("PCI.get_device_name", new JArray(session, _pci ?? ""), serializer);
        }

        public XenRef<Host> pci_get_host(string session, string _pci)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Host>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Host>>("PCI.get_host", new JArray(session, _pci ?? ""), serializer);
        }

        public string pci_get_pci_id(string session, string _pci)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("PCI.get_pci_id", new JArray(session, _pci ?? ""), serializer);
        }

        public List<XenRef<PCI>> pci_get_dependencies(string session, string _pci)
        {
            var converters = new List<JsonConverter> {new XenRefListConverter<PCI>()};
            var serializer = CreateSerializer(converters);
            return Rpc<List<XenRef<PCI>>>("PCI.get_dependencies", new JArray(session, _pci ?? ""), serializer);
        }

        public Dictionary<string, string> pci_get_other_config(string session, string _pci)
        {
            var converters = new List<JsonConverter> {new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<string, string>>("PCI.get_other_config", new JArray(session, _pci ?? ""), serializer);
        }

        public string pci_get_subsystem_vendor_name(string session, string _pci)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("PCI.get_subsystem_vendor_name", new JArray(session, _pci ?? ""), serializer);
        }

        public string pci_get_subsystem_device_name(string session, string _pci)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("PCI.get_subsystem_device_name", new JArray(session, _pci ?? ""), serializer);
        }

        public string pci_get_driver_name(string session, string _pci)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("PCI.get_driver_name", new JArray(session, _pci ?? ""), serializer);
        }

        public void pci_set_other_config(string session, string _pci, Dictionary<string, string> _other_config)
        {
            var converters = new List<JsonConverter> {new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            Rpc("PCI.set_other_config", new JArray(session, _pci ?? "", _other_config == null ? new JObject() : JObject.FromObject(_other_config, serializer)), serializer);
        }

        public void pci_add_to_other_config(string session, string _pci, string _key, string _value)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("PCI.add_to_other_config", new JArray(session, _pci ?? "", _key ?? "", _value ?? ""), serializer);
        }

        public void pci_remove_from_other_config(string session, string _pci, string _key)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("PCI.remove_from_other_config", new JArray(session, _pci ?? "", _key ?? ""), serializer);
        }

        public List<XenRef<PCI>> pci_get_all(string session)
        {
            var converters = new List<JsonConverter> {new XenRefListConverter<PCI>()};
            var serializer = CreateSerializer(converters);
            return Rpc<List<XenRef<PCI>>>("PCI.get_all", new JArray(session), serializer);
        }

        public Dictionary<XenRef<PCI>, PCI> pci_get_all_records(string session)
        {
            var converters = new List<JsonConverter> {new XenRefXenObjectMapConverter<PCI>()};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<XenRef<PCI>, PCI>>("PCI.get_all_records", new JArray(session), serializer);
        }

        public PGPU pgpu_get_record(string session, string _pgpu)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<PGPU>("PGPU.get_record", new JArray(session, _pgpu ?? ""), serializer);
        }

        public XenRef<PGPU> pgpu_get_by_uuid(string session, string _uuid)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<PGPU>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<PGPU>>("PGPU.get_by_uuid", new JArray(session, _uuid ?? ""), serializer);
        }

        public string pgpu_get_uuid(string session, string _pgpu)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("PGPU.get_uuid", new JArray(session, _pgpu ?? ""), serializer);
        }

        public XenRef<PCI> pgpu_get_pci(string session, string _pgpu)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<PCI>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<PCI>>("PGPU.get_PCI", new JArray(session, _pgpu ?? ""), serializer);
        }

        public XenRef<GPU_group> pgpu_get_gpu_group(string session, string _pgpu)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<GPU_group>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<GPU_group>>("PGPU.get_GPU_group", new JArray(session, _pgpu ?? ""), serializer);
        }

        public XenRef<Host> pgpu_get_host(string session, string _pgpu)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Host>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Host>>("PGPU.get_host", new JArray(session, _pgpu ?? ""), serializer);
        }

        public Dictionary<string, string> pgpu_get_other_config(string session, string _pgpu)
        {
            var converters = new List<JsonConverter> {new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<string, string>>("PGPU.get_other_config", new JArray(session, _pgpu ?? ""), serializer);
        }

        public List<XenRef<VGPU_type>> pgpu_get_supported_vgpu_types(string session, string _pgpu)
        {
            var converters = new List<JsonConverter> {new XenRefListConverter<VGPU_type>()};
            var serializer = CreateSerializer(converters);
            return Rpc<List<XenRef<VGPU_type>>>("PGPU.get_supported_VGPU_types", new JArray(session, _pgpu ?? ""), serializer);
        }

        public List<XenRef<VGPU_type>> pgpu_get_enabled_vgpu_types(string session, string _pgpu)
        {
            var converters = new List<JsonConverter> {new XenRefListConverter<VGPU_type>()};
            var serializer = CreateSerializer(converters);
            return Rpc<List<XenRef<VGPU_type>>>("PGPU.get_enabled_VGPU_types", new JArray(session, _pgpu ?? ""), serializer);
        }

        public List<XenRef<VGPU>> pgpu_get_resident_vgpus(string session, string _pgpu)
        {
            var converters = new List<JsonConverter> {new XenRefListConverter<VGPU>()};
            var serializer = CreateSerializer(converters);
            return Rpc<List<XenRef<VGPU>>>("PGPU.get_resident_VGPUs", new JArray(session, _pgpu ?? ""), serializer);
        }

        public Dictionary<XenRef<VGPU_type>, long> pgpu_get_supported_vgpu_max_capacities(string session, string _pgpu)
        {
            var converters = new List<JsonConverter> {new XenRefLongMapConverter<VGPU_type>()};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<XenRef<VGPU_type>, long>>("PGPU.get_supported_VGPU_max_capacities", new JArray(session, _pgpu ?? ""), serializer);
        }

        public pgpu_dom0_access pgpu_get_dom0_access(string session, string _pgpu)
        {
            var converters = new List<JsonConverter> {new pgpu_dom0_accessConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<pgpu_dom0_access>("PGPU.get_dom0_access", new JArray(session, _pgpu ?? ""), serializer);
        }

        public bool pgpu_get_is_system_display_device(string session, string _pgpu)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<bool>("PGPU.get_is_system_display_device", new JArray(session, _pgpu ?? ""), serializer);
        }

        public Dictionary<string, string> pgpu_get_compatibility_metadata(string session, string _pgpu)
        {
            var converters = new List<JsonConverter> {new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<string, string>>("PGPU.get_compatibility_metadata", new JArray(session, _pgpu ?? ""), serializer);
        }

        public void pgpu_set_other_config(string session, string _pgpu, Dictionary<string, string> _other_config)
        {
            var converters = new List<JsonConverter> {new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            Rpc("PGPU.set_other_config", new JArray(session, _pgpu ?? "", _other_config == null ? new JObject() : JObject.FromObject(_other_config, serializer)), serializer);
        }

        public void pgpu_add_to_other_config(string session, string _pgpu, string _key, string _value)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("PGPU.add_to_other_config", new JArray(session, _pgpu ?? "", _key ?? "", _value ?? ""), serializer);
        }

        public void pgpu_remove_from_other_config(string session, string _pgpu, string _key)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("PGPU.remove_from_other_config", new JArray(session, _pgpu ?? "", _key ?? ""), serializer);
        }

        public void pgpu_add_enabled_vgpu_types(string session, string _pgpu, string _value)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<VGPU_type>()};
            var serializer = CreateSerializer(converters);
            Rpc("PGPU.add_enabled_VGPU_types", new JArray(session, _pgpu ?? "", _value ?? ""), serializer);
        }

        public XenRef<Task> async_pgpu_add_enabled_vgpu_types(string session, string _pgpu, string _value)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>(), new XenRefConverter<VGPU_type>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.PGPU.add_enabled_VGPU_types", new JArray(session, _pgpu ?? "", _value ?? ""), serializer);
        }

        public void pgpu_remove_enabled_vgpu_types(string session, string _pgpu, string _value)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<VGPU_type>()};
            var serializer = CreateSerializer(converters);
            Rpc("PGPU.remove_enabled_VGPU_types", new JArray(session, _pgpu ?? "", _value ?? ""), serializer);
        }

        public XenRef<Task> async_pgpu_remove_enabled_vgpu_types(string session, string _pgpu, string _value)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>(), new XenRefConverter<VGPU_type>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.PGPU.remove_enabled_VGPU_types", new JArray(session, _pgpu ?? "", _value ?? ""), serializer);
        }

        public void pgpu_set_enabled_vgpu_types(string session, string _pgpu, List<XenRef<VGPU_type>> _value)
        {
            var converters = new List<JsonConverter> {new XenRefListConverter<VGPU_type>()};
            var serializer = CreateSerializer(converters);
            Rpc("PGPU.set_enabled_VGPU_types", new JArray(session, _pgpu ?? "", _value == null ? new JArray() : JArray.FromObject(_value, serializer)), serializer);
        }

        public XenRef<Task> async_pgpu_set_enabled_vgpu_types(string session, string _pgpu, List<XenRef<VGPU_type>> _value)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>(), new XenRefListConverter<VGPU_type>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.PGPU.set_enabled_VGPU_types", new JArray(session, _pgpu ?? "", _value == null ? new JArray() : JArray.FromObject(_value, serializer)), serializer);
        }

        public void pgpu_set_gpu_group(string session, string _pgpu, string _value)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<GPU_group>()};
            var serializer = CreateSerializer(converters);
            Rpc("PGPU.set_GPU_group", new JArray(session, _pgpu ?? "", _value ?? ""), serializer);
        }

        public XenRef<Task> async_pgpu_set_gpu_group(string session, string _pgpu, string _value)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>(), new XenRefConverter<GPU_group>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.PGPU.set_GPU_group", new JArray(session, _pgpu ?? "", _value ?? ""), serializer);
        }

        public long pgpu_get_remaining_capacity(string session, string _pgpu, string _vgpu_type)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<VGPU_type>()};
            var serializer = CreateSerializer(converters);
            return Rpc<long>("PGPU.get_remaining_capacity", new JArray(session, _pgpu ?? "", _vgpu_type ?? ""), serializer);
        }

        public XenRef<Task> async_pgpu_get_remaining_capacity(string session, string _pgpu, string _vgpu_type)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>(), new XenRefConverter<VGPU_type>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.PGPU.get_remaining_capacity", new JArray(session, _pgpu ?? "", _vgpu_type ?? ""), serializer);
        }

        public pgpu_dom0_access pgpu_enable_dom0_access(string session, string _pgpu)
        {
            var converters = new List<JsonConverter> {new pgpu_dom0_accessConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<pgpu_dom0_access>("PGPU.enable_dom0_access", new JArray(session, _pgpu ?? ""), serializer);
        }

        public XenRef<Task> async_pgpu_enable_dom0_access(string session, string _pgpu)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.PGPU.enable_dom0_access", new JArray(session, _pgpu ?? ""), serializer);
        }

        public pgpu_dom0_access pgpu_disable_dom0_access(string session, string _pgpu)
        {
            var converters = new List<JsonConverter> {new pgpu_dom0_accessConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<pgpu_dom0_access>("PGPU.disable_dom0_access", new JArray(session, _pgpu ?? ""), serializer);
        }

        public XenRef<Task> async_pgpu_disable_dom0_access(string session, string _pgpu)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.PGPU.disable_dom0_access", new JArray(session, _pgpu ?? ""), serializer);
        }

        public List<XenRef<PGPU>> pgpu_get_all(string session)
        {
            var converters = new List<JsonConverter> {new XenRefListConverter<PGPU>()};
            var serializer = CreateSerializer(converters);
            return Rpc<List<XenRef<PGPU>>>("PGPU.get_all", new JArray(session), serializer);
        }

        public Dictionary<XenRef<PGPU>, PGPU> pgpu_get_all_records(string session)
        {
            var converters = new List<JsonConverter> {new XenRefXenObjectMapConverter<PGPU>()};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<XenRef<PGPU>, PGPU>>("PGPU.get_all_records", new JArray(session), serializer);
        }

        public GPU_group gpu_group_get_record(string session, string _gpu_group)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<GPU_group>("GPU_group.get_record", new JArray(session, _gpu_group ?? ""), serializer);
        }

        public XenRef<GPU_group> gpu_group_get_by_uuid(string session, string _uuid)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<GPU_group>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<GPU_group>>("GPU_group.get_by_uuid", new JArray(session, _uuid ?? ""), serializer);
        }

        public List<XenRef<GPU_group>> gpu_group_get_by_name_label(string session, string _label)
        {
            var converters = new List<JsonConverter> {new XenRefListConverter<GPU_group>()};
            var serializer = CreateSerializer(converters);
            return Rpc<List<XenRef<GPU_group>>>("GPU_group.get_by_name_label", new JArray(session, _label ?? ""), serializer);
        }

        public string gpu_group_get_uuid(string session, string _gpu_group)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("GPU_group.get_uuid", new JArray(session, _gpu_group ?? ""), serializer);
        }

        public string gpu_group_get_name_label(string session, string _gpu_group)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("GPU_group.get_name_label", new JArray(session, _gpu_group ?? ""), serializer);
        }

        public string gpu_group_get_name_description(string session, string _gpu_group)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("GPU_group.get_name_description", new JArray(session, _gpu_group ?? ""), serializer);
        }

        public List<XenRef<PGPU>> gpu_group_get_pgpus(string session, string _gpu_group)
        {
            var converters = new List<JsonConverter> {new XenRefListConverter<PGPU>()};
            var serializer = CreateSerializer(converters);
            return Rpc<List<XenRef<PGPU>>>("GPU_group.get_PGPUs", new JArray(session, _gpu_group ?? ""), serializer);
        }

        public List<XenRef<VGPU>> gpu_group_get_vgpus(string session, string _gpu_group)
        {
            var converters = new List<JsonConverter> {new XenRefListConverter<VGPU>()};
            var serializer = CreateSerializer(converters);
            return Rpc<List<XenRef<VGPU>>>("GPU_group.get_VGPUs", new JArray(session, _gpu_group ?? ""), serializer);
        }

        public string[] gpu_group_get_gpu_types(string session, string _gpu_group)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string[]>("GPU_group.get_GPU_types", new JArray(session, _gpu_group ?? ""), serializer);
        }

        public Dictionary<string, string> gpu_group_get_other_config(string session, string _gpu_group)
        {
            var converters = new List<JsonConverter> {new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<string, string>>("GPU_group.get_other_config", new JArray(session, _gpu_group ?? ""), serializer);
        }

        public allocation_algorithm gpu_group_get_allocation_algorithm(string session, string _gpu_group)
        {
            var converters = new List<JsonConverter> {new allocation_algorithmConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<allocation_algorithm>("GPU_group.get_allocation_algorithm", new JArray(session, _gpu_group ?? ""), serializer);
        }

        public List<XenRef<VGPU_type>> gpu_group_get_supported_vgpu_types(string session, string _gpu_group)
        {
            var converters = new List<JsonConverter> {new XenRefListConverter<VGPU_type>()};
            var serializer = CreateSerializer(converters);
            return Rpc<List<XenRef<VGPU_type>>>("GPU_group.get_supported_VGPU_types", new JArray(session, _gpu_group ?? ""), serializer);
        }

        public List<XenRef<VGPU_type>> gpu_group_get_enabled_vgpu_types(string session, string _gpu_group)
        {
            var converters = new List<JsonConverter> {new XenRefListConverter<VGPU_type>()};
            var serializer = CreateSerializer(converters);
            return Rpc<List<XenRef<VGPU_type>>>("GPU_group.get_enabled_VGPU_types", new JArray(session, _gpu_group ?? ""), serializer);
        }

        public void gpu_group_set_name_label(string session, string _gpu_group, string _label)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("GPU_group.set_name_label", new JArray(session, _gpu_group ?? "", _label ?? ""), serializer);
        }

        public void gpu_group_set_name_description(string session, string _gpu_group, string _description)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("GPU_group.set_name_description", new JArray(session, _gpu_group ?? "", _description ?? ""), serializer);
        }

        public void gpu_group_set_other_config(string session, string _gpu_group, Dictionary<string, string> _other_config)
        {
            var converters = new List<JsonConverter> {new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            Rpc("GPU_group.set_other_config", new JArray(session, _gpu_group ?? "", _other_config == null ? new JObject() : JObject.FromObject(_other_config, serializer)), serializer);
        }

        public void gpu_group_add_to_other_config(string session, string _gpu_group, string _key, string _value)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("GPU_group.add_to_other_config", new JArray(session, _gpu_group ?? "", _key ?? "", _value ?? ""), serializer);
        }

        public void gpu_group_remove_from_other_config(string session, string _gpu_group, string _key)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("GPU_group.remove_from_other_config", new JArray(session, _gpu_group ?? "", _key ?? ""), serializer);
        }

        public void gpu_group_set_allocation_algorithm(string session, string _gpu_group, allocation_algorithm _allocation_algorithm)
        {
            var converters = new List<JsonConverter> {new allocation_algorithmConverter()};
            var serializer = CreateSerializer(converters);
            Rpc("GPU_group.set_allocation_algorithm", new JArray(session, _gpu_group ?? "", _allocation_algorithm.StringOf()), serializer);
        }

        public XenRef<GPU_group> gpu_group_create(string session, string _name_label, string _name_description, Dictionary<string, string> _other_config)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<GPU_group>(), new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<GPU_group>>("GPU_group.create", new JArray(session, _name_label ?? "", _name_description ?? "", _other_config == null ? new JObject() : JObject.FromObject(_other_config, serializer)), serializer);
        }

        public XenRef<Task> async_gpu_group_create(string session, string _name_label, string _name_description, Dictionary<string, string> _other_config)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>(), new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.GPU_group.create", new JArray(session, _name_label ?? "", _name_description ?? "", _other_config == null ? new JObject() : JObject.FromObject(_other_config, serializer)), serializer);
        }

        public void gpu_group_destroy(string session, string _gpu_group)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("GPU_group.destroy", new JArray(session, _gpu_group ?? ""), serializer);
        }

        public XenRef<Task> async_gpu_group_destroy(string session, string _gpu_group)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.GPU_group.destroy", new JArray(session, _gpu_group ?? ""), serializer);
        }

        public long gpu_group_get_remaining_capacity(string session, string _gpu_group, string _vgpu_type)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<VGPU_type>()};
            var serializer = CreateSerializer(converters);
            return Rpc<long>("GPU_group.get_remaining_capacity", new JArray(session, _gpu_group ?? "", _vgpu_type ?? ""), serializer);
        }

        public XenRef<Task> async_gpu_group_get_remaining_capacity(string session, string _gpu_group, string _vgpu_type)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>(), new XenRefConverter<VGPU_type>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.GPU_group.get_remaining_capacity", new JArray(session, _gpu_group ?? "", _vgpu_type ?? ""), serializer);
        }

        public List<XenRef<GPU_group>> gpu_group_get_all(string session)
        {
            var converters = new List<JsonConverter> {new XenRefListConverter<GPU_group>()};
            var serializer = CreateSerializer(converters);
            return Rpc<List<XenRef<GPU_group>>>("GPU_group.get_all", new JArray(session), serializer);
        }

        public Dictionary<XenRef<GPU_group>, GPU_group> gpu_group_get_all_records(string session)
        {
            var converters = new List<JsonConverter> {new XenRefXenObjectMapConverter<GPU_group>()};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<XenRef<GPU_group>, GPU_group>>("GPU_group.get_all_records", new JArray(session), serializer);
        }

        public VGPU vgpu_get_record(string session, string _vgpu)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<VGPU>("VGPU.get_record", new JArray(session, _vgpu ?? ""), serializer);
        }

        public XenRef<VGPU> vgpu_get_by_uuid(string session, string _uuid)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<VGPU>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<VGPU>>("VGPU.get_by_uuid", new JArray(session, _uuid ?? ""), serializer);
        }

        public string vgpu_get_uuid(string session, string _vgpu)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("VGPU.get_uuid", new JArray(session, _vgpu ?? ""), serializer);
        }

        public XenRef<VM> vgpu_get_vm(string session, string _vgpu)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<VM>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<VM>>("VGPU.get_VM", new JArray(session, _vgpu ?? ""), serializer);
        }

        public XenRef<GPU_group> vgpu_get_gpu_group(string session, string _vgpu)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<GPU_group>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<GPU_group>>("VGPU.get_GPU_group", new JArray(session, _vgpu ?? ""), serializer);
        }

        public string vgpu_get_device(string session, string _vgpu)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("VGPU.get_device", new JArray(session, _vgpu ?? ""), serializer);
        }

        public bool vgpu_get_currently_attached(string session, string _vgpu)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<bool>("VGPU.get_currently_attached", new JArray(session, _vgpu ?? ""), serializer);
        }

        public Dictionary<string, string> vgpu_get_other_config(string session, string _vgpu)
        {
            var converters = new List<JsonConverter> {new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<string, string>>("VGPU.get_other_config", new JArray(session, _vgpu ?? ""), serializer);
        }

        public XenRef<VGPU_type> vgpu_get_type(string session, string _vgpu)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<VGPU_type>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<VGPU_type>>("VGPU.get_type", new JArray(session, _vgpu ?? ""), serializer);
        }

        public XenRef<PGPU> vgpu_get_resident_on(string session, string _vgpu)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<PGPU>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<PGPU>>("VGPU.get_resident_on", new JArray(session, _vgpu ?? ""), serializer);
        }

        public XenRef<PGPU> vgpu_get_scheduled_to_be_resident_on(string session, string _vgpu)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<PGPU>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<PGPU>>("VGPU.get_scheduled_to_be_resident_on", new JArray(session, _vgpu ?? ""), serializer);
        }

        public Dictionary<string, string> vgpu_get_compatibility_metadata(string session, string _vgpu)
        {
            var converters = new List<JsonConverter> {new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<string, string>>("VGPU.get_compatibility_metadata", new JArray(session, _vgpu ?? ""), serializer);
        }

        public void vgpu_set_other_config(string session, string _vgpu, Dictionary<string, string> _other_config)
        {
            var converters = new List<JsonConverter> {new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            Rpc("VGPU.set_other_config", new JArray(session, _vgpu ?? "", _other_config == null ? new JObject() : JObject.FromObject(_other_config, serializer)), serializer);
        }

        public void vgpu_add_to_other_config(string session, string _vgpu, string _key, string _value)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("VGPU.add_to_other_config", new JArray(session, _vgpu ?? "", _key ?? "", _value ?? ""), serializer);
        }

        public void vgpu_remove_from_other_config(string session, string _vgpu, string _key)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("VGPU.remove_from_other_config", new JArray(session, _vgpu ?? "", _key ?? ""), serializer);
        }

        public XenRef<VGPU> vgpu_create(string session, string _vm, string _gpu_group, string _device, Dictionary<string, string> _other_config)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<VGPU>(), new XenRefConverter<VM>(), new XenRefConverter<GPU_group>(), new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<VGPU>>("VGPU.create", new JArray(session, _vm ?? "", _gpu_group ?? "", _device ?? "", _other_config == null ? new JObject() : JObject.FromObject(_other_config, serializer)), serializer);
        }

        public XenRef<Task> async_vgpu_create(string session, string _vm, string _gpu_group, string _device, Dictionary<string, string> _other_config)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>(), new XenRefConverter<VM>(), new XenRefConverter<GPU_group>(), new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.VGPU.create", new JArray(session, _vm ?? "", _gpu_group ?? "", _device ?? "", _other_config == null ? new JObject() : JObject.FromObject(_other_config, serializer)), serializer);
        }

        public XenRef<VGPU> vgpu_create(string session, string _vm, string _gpu_group, string _device, Dictionary<string, string> _other_config, string _type)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<VGPU>(), new XenRefConverter<VM>(), new XenRefConverter<GPU_group>(), new StringStringMapConverter(), new XenRefConverter<VGPU_type>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<VGPU>>("VGPU.create", new JArray(session, _vm ?? "", _gpu_group ?? "", _device ?? "", _other_config == null ? new JObject() : JObject.FromObject(_other_config, serializer), _type ?? ""), serializer);
        }

        public XenRef<Task> async_vgpu_create(string session, string _vm, string _gpu_group, string _device, Dictionary<string, string> _other_config, string _type)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>(), new XenRefConverter<VM>(), new XenRefConverter<GPU_group>(), new StringStringMapConverter(), new XenRefConverter<VGPU_type>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.VGPU.create", new JArray(session, _vm ?? "", _gpu_group ?? "", _device ?? "", _other_config == null ? new JObject() : JObject.FromObject(_other_config, serializer), _type ?? ""), serializer);
        }

        public void vgpu_destroy(string session, string _vgpu)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("VGPU.destroy", new JArray(session, _vgpu ?? ""), serializer);
        }

        public XenRef<Task> async_vgpu_destroy(string session, string _vgpu)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.VGPU.destroy", new JArray(session, _vgpu ?? ""), serializer);
        }

        public List<XenRef<VGPU>> vgpu_get_all(string session)
        {
            var converters = new List<JsonConverter> {new XenRefListConverter<VGPU>()};
            var serializer = CreateSerializer(converters);
            return Rpc<List<XenRef<VGPU>>>("VGPU.get_all", new JArray(session), serializer);
        }

        public Dictionary<XenRef<VGPU>, VGPU> vgpu_get_all_records(string session)
        {
            var converters = new List<JsonConverter> {new XenRefXenObjectMapConverter<VGPU>()};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<XenRef<VGPU>, VGPU>>("VGPU.get_all_records", new JArray(session), serializer);
        }

        public VGPU_type vgpu_type_get_record(string session, string _vgpu_type)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<VGPU_type>("VGPU_type.get_record", new JArray(session, _vgpu_type ?? ""), serializer);
        }

        public XenRef<VGPU_type> vgpu_type_get_by_uuid(string session, string _uuid)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<VGPU_type>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<VGPU_type>>("VGPU_type.get_by_uuid", new JArray(session, _uuid ?? ""), serializer);
        }

        public string vgpu_type_get_uuid(string session, string _vgpu_type)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("VGPU_type.get_uuid", new JArray(session, _vgpu_type ?? ""), serializer);
        }

        public string vgpu_type_get_vendor_name(string session, string _vgpu_type)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("VGPU_type.get_vendor_name", new JArray(session, _vgpu_type ?? ""), serializer);
        }

        public string vgpu_type_get_model_name(string session, string _vgpu_type)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("VGPU_type.get_model_name", new JArray(session, _vgpu_type ?? ""), serializer);
        }

        public long vgpu_type_get_framebuffer_size(string session, string _vgpu_type)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<long>("VGPU_type.get_framebuffer_size", new JArray(session, _vgpu_type ?? ""), serializer);
        }

        public long vgpu_type_get_max_heads(string session, string _vgpu_type)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<long>("VGPU_type.get_max_heads", new JArray(session, _vgpu_type ?? ""), serializer);
        }

        public long vgpu_type_get_max_resolution_x(string session, string _vgpu_type)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<long>("VGPU_type.get_max_resolution_x", new JArray(session, _vgpu_type ?? ""), serializer);
        }

        public long vgpu_type_get_max_resolution_y(string session, string _vgpu_type)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<long>("VGPU_type.get_max_resolution_y", new JArray(session, _vgpu_type ?? ""), serializer);
        }

        public List<XenRef<PGPU>> vgpu_type_get_supported_on_pgpus(string session, string _vgpu_type)
        {
            var converters = new List<JsonConverter> {new XenRefListConverter<PGPU>()};
            var serializer = CreateSerializer(converters);
            return Rpc<List<XenRef<PGPU>>>("VGPU_type.get_supported_on_PGPUs", new JArray(session, _vgpu_type ?? ""), serializer);
        }

        public List<XenRef<PGPU>> vgpu_type_get_enabled_on_pgpus(string session, string _vgpu_type)
        {
            var converters = new List<JsonConverter> {new XenRefListConverter<PGPU>()};
            var serializer = CreateSerializer(converters);
            return Rpc<List<XenRef<PGPU>>>("VGPU_type.get_enabled_on_PGPUs", new JArray(session, _vgpu_type ?? ""), serializer);
        }

        public List<XenRef<VGPU>> vgpu_type_get_vgpus(string session, string _vgpu_type)
        {
            var converters = new List<JsonConverter> {new XenRefListConverter<VGPU>()};
            var serializer = CreateSerializer(converters);
            return Rpc<List<XenRef<VGPU>>>("VGPU_type.get_VGPUs", new JArray(session, _vgpu_type ?? ""), serializer);
        }

        public List<XenRef<GPU_group>> vgpu_type_get_supported_on_gpu_groups(string session, string _vgpu_type)
        {
            var converters = new List<JsonConverter> {new XenRefListConverter<GPU_group>()};
            var serializer = CreateSerializer(converters);
            return Rpc<List<XenRef<GPU_group>>>("VGPU_type.get_supported_on_GPU_groups", new JArray(session, _vgpu_type ?? ""), serializer);
        }

        public List<XenRef<GPU_group>> vgpu_type_get_enabled_on_gpu_groups(string session, string _vgpu_type)
        {
            var converters = new List<JsonConverter> {new XenRefListConverter<GPU_group>()};
            var serializer = CreateSerializer(converters);
            return Rpc<List<XenRef<GPU_group>>>("VGPU_type.get_enabled_on_GPU_groups", new JArray(session, _vgpu_type ?? ""), serializer);
        }

        public vgpu_type_implementation vgpu_type_get_implementation(string session, string _vgpu_type)
        {
            var converters = new List<JsonConverter> {new vgpu_type_implementationConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<vgpu_type_implementation>("VGPU_type.get_implementation", new JArray(session, _vgpu_type ?? ""), serializer);
        }

        public string vgpu_type_get_identifier(string session, string _vgpu_type)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("VGPU_type.get_identifier", new JArray(session, _vgpu_type ?? ""), serializer);
        }

        public bool vgpu_type_get_experimental(string session, string _vgpu_type)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<bool>("VGPU_type.get_experimental", new JArray(session, _vgpu_type ?? ""), serializer);
        }

        public List<XenRef<VGPU_type>> vgpu_type_get_all(string session)
        {
            var converters = new List<JsonConverter> {new XenRefListConverter<VGPU_type>()};
            var serializer = CreateSerializer(converters);
            return Rpc<List<XenRef<VGPU_type>>>("VGPU_type.get_all", new JArray(session), serializer);
        }

        public Dictionary<XenRef<VGPU_type>, VGPU_type> vgpu_type_get_all_records(string session)
        {
            var converters = new List<JsonConverter> {new XenRefXenObjectMapConverter<VGPU_type>()};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<XenRef<VGPU_type>, VGPU_type>>("VGPU_type.get_all_records", new JArray(session), serializer);
        }

        public PVS_site pvs_site_get_record(string session, string _pvs_site)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<PVS_site>("PVS_site.get_record", new JArray(session, _pvs_site ?? ""), serializer);
        }

        public XenRef<PVS_site> pvs_site_get_by_uuid(string session, string _uuid)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<PVS_site>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<PVS_site>>("PVS_site.get_by_uuid", new JArray(session, _uuid ?? ""), serializer);
        }

        public List<XenRef<PVS_site>> pvs_site_get_by_name_label(string session, string _label)
        {
            var converters = new List<JsonConverter> {new XenRefListConverter<PVS_site>()};
            var serializer = CreateSerializer(converters);
            return Rpc<List<XenRef<PVS_site>>>("PVS_site.get_by_name_label", new JArray(session, _label ?? ""), serializer);
        }

        public string pvs_site_get_uuid(string session, string _pvs_site)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("PVS_site.get_uuid", new JArray(session, _pvs_site ?? ""), serializer);
        }

        public string pvs_site_get_name_label(string session, string _pvs_site)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("PVS_site.get_name_label", new JArray(session, _pvs_site ?? ""), serializer);
        }

        public string pvs_site_get_name_description(string session, string _pvs_site)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("PVS_site.get_name_description", new JArray(session, _pvs_site ?? ""), serializer);
        }

        public string pvs_site_get_pvs_uuid(string session, string _pvs_site)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("PVS_site.get_PVS_uuid", new JArray(session, _pvs_site ?? ""), serializer);
        }

        public List<XenRef<PVS_cache_storage>> pvs_site_get_cache_storage(string session, string _pvs_site)
        {
            var converters = new List<JsonConverter> {new XenRefListConverter<PVS_cache_storage>()};
            var serializer = CreateSerializer(converters);
            return Rpc<List<XenRef<PVS_cache_storage>>>("PVS_site.get_cache_storage", new JArray(session, _pvs_site ?? ""), serializer);
        }

        public List<XenRef<PVS_server>> pvs_site_get_servers(string session, string _pvs_site)
        {
            var converters = new List<JsonConverter> {new XenRefListConverter<PVS_server>()};
            var serializer = CreateSerializer(converters);
            return Rpc<List<XenRef<PVS_server>>>("PVS_site.get_servers", new JArray(session, _pvs_site ?? ""), serializer);
        }

        public List<XenRef<PVS_proxy>> pvs_site_get_proxies(string session, string _pvs_site)
        {
            var converters = new List<JsonConverter> {new XenRefListConverter<PVS_proxy>()};
            var serializer = CreateSerializer(converters);
            return Rpc<List<XenRef<PVS_proxy>>>("PVS_site.get_proxies", new JArray(session, _pvs_site ?? ""), serializer);
        }

        public void pvs_site_set_name_label(string session, string _pvs_site, string _label)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("PVS_site.set_name_label", new JArray(session, _pvs_site ?? "", _label ?? ""), serializer);
        }

        public void pvs_site_set_name_description(string session, string _pvs_site, string _description)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("PVS_site.set_name_description", new JArray(session, _pvs_site ?? "", _description ?? ""), serializer);
        }

        public XenRef<PVS_site> pvs_site_introduce(string session, string _name_label, string _name_description, string _pvs_uuid)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<PVS_site>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<PVS_site>>("PVS_site.introduce", new JArray(session, _name_label ?? "", _name_description ?? "", _pvs_uuid ?? ""), serializer);
        }

        public XenRef<Task> async_pvs_site_introduce(string session, string _name_label, string _name_description, string _pvs_uuid)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.PVS_site.introduce", new JArray(session, _name_label ?? "", _name_description ?? "", _pvs_uuid ?? ""), serializer);
        }

        public void pvs_site_forget(string session, string _pvs_site)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("PVS_site.forget", new JArray(session, _pvs_site ?? ""), serializer);
        }

        public XenRef<Task> async_pvs_site_forget(string session, string _pvs_site)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.PVS_site.forget", new JArray(session, _pvs_site ?? ""), serializer);
        }

        public void pvs_site_set_pvs_uuid(string session, string _pvs_site, string _value)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("PVS_site.set_PVS_uuid", new JArray(session, _pvs_site ?? "", _value ?? ""), serializer);
        }

        public XenRef<Task> async_pvs_site_set_pvs_uuid(string session, string _pvs_site, string _value)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.PVS_site.set_PVS_uuid", new JArray(session, _pvs_site ?? "", _value ?? ""), serializer);
        }

        public List<XenRef<PVS_site>> pvs_site_get_all(string session)
        {
            var converters = new List<JsonConverter> {new XenRefListConverter<PVS_site>()};
            var serializer = CreateSerializer(converters);
            return Rpc<List<XenRef<PVS_site>>>("PVS_site.get_all", new JArray(session), serializer);
        }

        public Dictionary<XenRef<PVS_site>, PVS_site> pvs_site_get_all_records(string session)
        {
            var converters = new List<JsonConverter> {new XenRefXenObjectMapConverter<PVS_site>()};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<XenRef<PVS_site>, PVS_site>>("PVS_site.get_all_records", new JArray(session), serializer);
        }

        public PVS_server pvs_server_get_record(string session, string _pvs_server)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<PVS_server>("PVS_server.get_record", new JArray(session, _pvs_server ?? ""), serializer);
        }

        public XenRef<PVS_server> pvs_server_get_by_uuid(string session, string _uuid)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<PVS_server>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<PVS_server>>("PVS_server.get_by_uuid", new JArray(session, _uuid ?? ""), serializer);
        }

        public string pvs_server_get_uuid(string session, string _pvs_server)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("PVS_server.get_uuid", new JArray(session, _pvs_server ?? ""), serializer);
        }

        public string[] pvs_server_get_addresses(string session, string _pvs_server)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string[]>("PVS_server.get_addresses", new JArray(session, _pvs_server ?? ""), serializer);
        }

        public long pvs_server_get_first_port(string session, string _pvs_server)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<long>("PVS_server.get_first_port", new JArray(session, _pvs_server ?? ""), serializer);
        }

        public long pvs_server_get_last_port(string session, string _pvs_server)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<long>("PVS_server.get_last_port", new JArray(session, _pvs_server ?? ""), serializer);
        }

        public XenRef<PVS_site> pvs_server_get_site(string session, string _pvs_server)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<PVS_site>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<PVS_site>>("PVS_server.get_site", new JArray(session, _pvs_server ?? ""), serializer);
        }

        public XenRef<PVS_server> pvs_server_introduce(string session, string[] _addresses, long _first_port, long _last_port, string _site)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<PVS_server>(), new XenRefConverter<PVS_site>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<PVS_server>>("PVS_server.introduce", new JArray(session, _addresses == null ? new JArray() : JArray.FromObject(_addresses), _first_port, _last_port, _site ?? ""), serializer);
        }

        public XenRef<Task> async_pvs_server_introduce(string session, string[] _addresses, long _first_port, long _last_port, string _site)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>(), new XenRefConverter<PVS_site>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.PVS_server.introduce", new JArray(session, _addresses == null ? new JArray() : JArray.FromObject(_addresses), _first_port, _last_port, _site ?? ""), serializer);
        }

        public void pvs_server_forget(string session, string _pvs_server)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("PVS_server.forget", new JArray(session, _pvs_server ?? ""), serializer);
        }

        public XenRef<Task> async_pvs_server_forget(string session, string _pvs_server)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.PVS_server.forget", new JArray(session, _pvs_server ?? ""), serializer);
        }

        public List<XenRef<PVS_server>> pvs_server_get_all(string session)
        {
            var converters = new List<JsonConverter> {new XenRefListConverter<PVS_server>()};
            var serializer = CreateSerializer(converters);
            return Rpc<List<XenRef<PVS_server>>>("PVS_server.get_all", new JArray(session), serializer);
        }

        public Dictionary<XenRef<PVS_server>, PVS_server> pvs_server_get_all_records(string session)
        {
            var converters = new List<JsonConverter> {new XenRefXenObjectMapConverter<PVS_server>()};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<XenRef<PVS_server>, PVS_server>>("PVS_server.get_all_records", new JArray(session), serializer);
        }

        public PVS_proxy pvs_proxy_get_record(string session, string _pvs_proxy)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<PVS_proxy>("PVS_proxy.get_record", new JArray(session, _pvs_proxy ?? ""), serializer);
        }

        public XenRef<PVS_proxy> pvs_proxy_get_by_uuid(string session, string _uuid)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<PVS_proxy>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<PVS_proxy>>("PVS_proxy.get_by_uuid", new JArray(session, _uuid ?? ""), serializer);
        }

        public string pvs_proxy_get_uuid(string session, string _pvs_proxy)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("PVS_proxy.get_uuid", new JArray(session, _pvs_proxy ?? ""), serializer);
        }

        public XenRef<PVS_site> pvs_proxy_get_site(string session, string _pvs_proxy)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<PVS_site>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<PVS_site>>("PVS_proxy.get_site", new JArray(session, _pvs_proxy ?? ""), serializer);
        }

        public XenRef<VIF> pvs_proxy_get_vif(string session, string _pvs_proxy)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<VIF>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<VIF>>("PVS_proxy.get_VIF", new JArray(session, _pvs_proxy ?? ""), serializer);
        }

        public bool pvs_proxy_get_currently_attached(string session, string _pvs_proxy)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<bool>("PVS_proxy.get_currently_attached", new JArray(session, _pvs_proxy ?? ""), serializer);
        }

        public pvs_proxy_status pvs_proxy_get_status(string session, string _pvs_proxy)
        {
            var converters = new List<JsonConverter> {new pvs_proxy_statusConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<pvs_proxy_status>("PVS_proxy.get_status", new JArray(session, _pvs_proxy ?? ""), serializer);
        }

        public XenRef<PVS_proxy> pvs_proxy_create(string session, string _site, string _vif)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<PVS_proxy>(), new XenRefConverter<PVS_site>(), new XenRefConverter<VIF>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<PVS_proxy>>("PVS_proxy.create", new JArray(session, _site ?? "", _vif ?? ""), serializer);
        }

        public XenRef<Task> async_pvs_proxy_create(string session, string _site, string _vif)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>(), new XenRefConverter<PVS_site>(), new XenRefConverter<VIF>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.PVS_proxy.create", new JArray(session, _site ?? "", _vif ?? ""), serializer);
        }

        public void pvs_proxy_destroy(string session, string _pvs_proxy)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("PVS_proxy.destroy", new JArray(session, _pvs_proxy ?? ""), serializer);
        }

        public XenRef<Task> async_pvs_proxy_destroy(string session, string _pvs_proxy)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.PVS_proxy.destroy", new JArray(session, _pvs_proxy ?? ""), serializer);
        }

        public List<XenRef<PVS_proxy>> pvs_proxy_get_all(string session)
        {
            var converters = new List<JsonConverter> {new XenRefListConverter<PVS_proxy>()};
            var serializer = CreateSerializer(converters);
            return Rpc<List<XenRef<PVS_proxy>>>("PVS_proxy.get_all", new JArray(session), serializer);
        }

        public Dictionary<XenRef<PVS_proxy>, PVS_proxy> pvs_proxy_get_all_records(string session)
        {
            var converters = new List<JsonConverter> {new XenRefXenObjectMapConverter<PVS_proxy>()};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<XenRef<PVS_proxy>, PVS_proxy>>("PVS_proxy.get_all_records", new JArray(session), serializer);
        }

        public PVS_cache_storage pvs_cache_storage_get_record(string session, string _pvs_cache_storage)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<PVS_cache_storage>("PVS_cache_storage.get_record", new JArray(session, _pvs_cache_storage ?? ""), serializer);
        }

        public XenRef<PVS_cache_storage> pvs_cache_storage_get_by_uuid(string session, string _uuid)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<PVS_cache_storage>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<PVS_cache_storage>>("PVS_cache_storage.get_by_uuid", new JArray(session, _uuid ?? ""), serializer);
        }

        public XenRef<PVS_cache_storage> pvs_cache_storage_create(string session, PVS_cache_storage _record)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<PVS_cache_storage>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<PVS_cache_storage>>("PVS_cache_storage.create", new JArray(session, _record.ToJObject()), serializer);
        }

        public XenRef<Task> async_pvs_cache_storage_create(string session, PVS_cache_storage _record)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.PVS_cache_storage.create", new JArray(session, _record.ToJObject()), serializer);
        }

        public void pvs_cache_storage_destroy(string session, string _pvs_cache_storage)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("PVS_cache_storage.destroy", new JArray(session, _pvs_cache_storage ?? ""), serializer);
        }

        public XenRef<Task> async_pvs_cache_storage_destroy(string session, string _pvs_cache_storage)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.PVS_cache_storage.destroy", new JArray(session, _pvs_cache_storage ?? ""), serializer);
        }

        public string pvs_cache_storage_get_uuid(string session, string _pvs_cache_storage)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("PVS_cache_storage.get_uuid", new JArray(session, _pvs_cache_storage ?? ""), serializer);
        }

        public XenRef<Host> pvs_cache_storage_get_host(string session, string _pvs_cache_storage)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Host>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Host>>("PVS_cache_storage.get_host", new JArray(session, _pvs_cache_storage ?? ""), serializer);
        }

        public XenRef<SR> pvs_cache_storage_get_sr(string session, string _pvs_cache_storage)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<SR>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<SR>>("PVS_cache_storage.get_SR", new JArray(session, _pvs_cache_storage ?? ""), serializer);
        }

        public XenRef<PVS_site> pvs_cache_storage_get_site(string session, string _pvs_cache_storage)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<PVS_site>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<PVS_site>>("PVS_cache_storage.get_site", new JArray(session, _pvs_cache_storage ?? ""), serializer);
        }

        public long pvs_cache_storage_get_size(string session, string _pvs_cache_storage)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<long>("PVS_cache_storage.get_size", new JArray(session, _pvs_cache_storage ?? ""), serializer);
        }

        public XenRef<VDI> pvs_cache_storage_get_vdi(string session, string _pvs_cache_storage)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<VDI>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<VDI>>("PVS_cache_storage.get_VDI", new JArray(session, _pvs_cache_storage ?? ""), serializer);
        }

        public List<XenRef<PVS_cache_storage>> pvs_cache_storage_get_all(string session)
        {
            var converters = new List<JsonConverter> {new XenRefListConverter<PVS_cache_storage>()};
            var serializer = CreateSerializer(converters);
            return Rpc<List<XenRef<PVS_cache_storage>>>("PVS_cache_storage.get_all", new JArray(session), serializer);
        }

        public Dictionary<XenRef<PVS_cache_storage>, PVS_cache_storage> pvs_cache_storage_get_all_records(string session)
        {
            var converters = new List<JsonConverter> {new XenRefXenObjectMapConverter<PVS_cache_storage>()};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<XenRef<PVS_cache_storage>, PVS_cache_storage>>("PVS_cache_storage.get_all_records", new JArray(session), serializer);
        }

        public Feature feature_get_record(string session, string _feature)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<Feature>("Feature.get_record", new JArray(session, _feature ?? ""), serializer);
        }

        public XenRef<Feature> feature_get_by_uuid(string session, string _uuid)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Feature>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Feature>>("Feature.get_by_uuid", new JArray(session, _uuid ?? ""), serializer);
        }

        public List<XenRef<Feature>> feature_get_by_name_label(string session, string _label)
        {
            var converters = new List<JsonConverter> {new XenRefListConverter<Feature>()};
            var serializer = CreateSerializer(converters);
            return Rpc<List<XenRef<Feature>>>("Feature.get_by_name_label", new JArray(session, _label ?? ""), serializer);
        }

        public string feature_get_uuid(string session, string _feature)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("Feature.get_uuid", new JArray(session, _feature ?? ""), serializer);
        }

        public string feature_get_name_label(string session, string _feature)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("Feature.get_name_label", new JArray(session, _feature ?? ""), serializer);
        }

        public string feature_get_name_description(string session, string _feature)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("Feature.get_name_description", new JArray(session, _feature ?? ""), serializer);
        }

        public bool feature_get_enabled(string session, string _feature)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<bool>("Feature.get_enabled", new JArray(session, _feature ?? ""), serializer);
        }

        public bool feature_get_experimental(string session, string _feature)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<bool>("Feature.get_experimental", new JArray(session, _feature ?? ""), serializer);
        }

        public string feature_get_version(string session, string _feature)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("Feature.get_version", new JArray(session, _feature ?? ""), serializer);
        }

        public XenRef<Host> feature_get_host(string session, string _feature)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Host>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Host>>("Feature.get_host", new JArray(session, _feature ?? ""), serializer);
        }

        public List<XenRef<Feature>> feature_get_all(string session)
        {
            var converters = new List<JsonConverter> {new XenRefListConverter<Feature>()};
            var serializer = CreateSerializer(converters);
            return Rpc<List<XenRef<Feature>>>("Feature.get_all", new JArray(session), serializer);
        }

        public Dictionary<XenRef<Feature>, Feature> feature_get_all_records(string session)
        {
            var converters = new List<JsonConverter> {new XenRefXenObjectMapConverter<Feature>()};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<XenRef<Feature>, Feature>>("Feature.get_all_records", new JArray(session), serializer);
        }

        public SDN_controller sdn_controller_get_record(string session, string _sdn_controller)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<SDN_controller>("SDN_controller.get_record", new JArray(session, _sdn_controller ?? ""), serializer);
        }

        public XenRef<SDN_controller> sdn_controller_get_by_uuid(string session, string _uuid)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<SDN_controller>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<SDN_controller>>("SDN_controller.get_by_uuid", new JArray(session, _uuid ?? ""), serializer);
        }

        public string sdn_controller_get_uuid(string session, string _sdn_controller)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("SDN_controller.get_uuid", new JArray(session, _sdn_controller ?? ""), serializer);
        }

        public sdn_controller_protocol sdn_controller_get_protocol(string session, string _sdn_controller)
        {
            var converters = new List<JsonConverter> {new sdn_controller_protocolConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<sdn_controller_protocol>("SDN_controller.get_protocol", new JArray(session, _sdn_controller ?? ""), serializer);
        }

        public string sdn_controller_get_address(string session, string _sdn_controller)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("SDN_controller.get_address", new JArray(session, _sdn_controller ?? ""), serializer);
        }

        public long sdn_controller_get_port(string session, string _sdn_controller)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<long>("SDN_controller.get_port", new JArray(session, _sdn_controller ?? ""), serializer);
        }

        public XenRef<SDN_controller> sdn_controller_introduce(string session, sdn_controller_protocol _protocol, string _address, long _port)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<SDN_controller>(), new sdn_controller_protocolConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<SDN_controller>>("SDN_controller.introduce", new JArray(session, _protocol.StringOf(), _address ?? "", _port), serializer);
        }

        public XenRef<Task> async_sdn_controller_introduce(string session, sdn_controller_protocol _protocol, string _address, long _port)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>(), new sdn_controller_protocolConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.SDN_controller.introduce", new JArray(session, _protocol.StringOf(), _address ?? "", _port), serializer);
        }

        public void sdn_controller_forget(string session, string _sdn_controller)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("SDN_controller.forget", new JArray(session, _sdn_controller ?? ""), serializer);
        }

        public XenRef<Task> async_sdn_controller_forget(string session, string _sdn_controller)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.SDN_controller.forget", new JArray(session, _sdn_controller ?? ""), serializer);
        }

        public List<XenRef<SDN_controller>> sdn_controller_get_all(string session)
        {
            var converters = new List<JsonConverter> {new XenRefListConverter<SDN_controller>()};
            var serializer = CreateSerializer(converters);
            return Rpc<List<XenRef<SDN_controller>>>("SDN_controller.get_all", new JArray(session), serializer);
        }

        public Dictionary<XenRef<SDN_controller>, SDN_controller> sdn_controller_get_all_records(string session)
        {
            var converters = new List<JsonConverter> {new XenRefXenObjectMapConverter<SDN_controller>()};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<XenRef<SDN_controller>, SDN_controller>>("SDN_controller.get_all_records", new JArray(session), serializer);
        }

        public Dictionary<XenRef<Vdi_nbd_server_info>, Vdi_nbd_server_info> vdi_nbd_server_info_get_all_records(string session)
        {
            var converters = new List<JsonConverter> {new XenRefXenObjectMapConverter<Vdi_nbd_server_info>()};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<XenRef<Vdi_nbd_server_info>, Vdi_nbd_server_info>>("vdi_nbd_server_info.get_all_records", new JArray(session), serializer);
        }

        public PUSB pusb_get_record(string session, string _pusb)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<PUSB>("PUSB.get_record", new JArray(session, _pusb ?? ""), serializer);
        }

        public XenRef<PUSB> pusb_get_by_uuid(string session, string _uuid)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<PUSB>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<PUSB>>("PUSB.get_by_uuid", new JArray(session, _uuid ?? ""), serializer);
        }

        public string pusb_get_uuid(string session, string _pusb)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("PUSB.get_uuid", new JArray(session, _pusb ?? ""), serializer);
        }

        public XenRef<USB_group> pusb_get_usb_group(string session, string _pusb)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<USB_group>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<USB_group>>("PUSB.get_USB_group", new JArray(session, _pusb ?? ""), serializer);
        }

        public XenRef<Host> pusb_get_host(string session, string _pusb)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Host>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Host>>("PUSB.get_host", new JArray(session, _pusb ?? ""), serializer);
        }

        public string pusb_get_path(string session, string _pusb)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("PUSB.get_path", new JArray(session, _pusb ?? ""), serializer);
        }

        public string pusb_get_vendor_id(string session, string _pusb)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("PUSB.get_vendor_id", new JArray(session, _pusb ?? ""), serializer);
        }

        public string pusb_get_vendor_desc(string session, string _pusb)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("PUSB.get_vendor_desc", new JArray(session, _pusb ?? ""), serializer);
        }

        public string pusb_get_product_id(string session, string _pusb)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("PUSB.get_product_id", new JArray(session, _pusb ?? ""), serializer);
        }

        public string pusb_get_product_desc(string session, string _pusb)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("PUSB.get_product_desc", new JArray(session, _pusb ?? ""), serializer);
        }

        public string pusb_get_serial(string session, string _pusb)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("PUSB.get_serial", new JArray(session, _pusb ?? ""), serializer);
        }

        public string pusb_get_version(string session, string _pusb)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("PUSB.get_version", new JArray(session, _pusb ?? ""), serializer);
        }

        public string pusb_get_description(string session, string _pusb)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("PUSB.get_description", new JArray(session, _pusb ?? ""), serializer);
        }

        public bool pusb_get_passthrough_enabled(string session, string _pusb)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<bool>("PUSB.get_passthrough_enabled", new JArray(session, _pusb ?? ""), serializer);
        }

        public Dictionary<string, string> pusb_get_other_config(string session, string _pusb)
        {
            var converters = new List<JsonConverter> {new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<string, string>>("PUSB.get_other_config", new JArray(session, _pusb ?? ""), serializer);
        }

        public void pusb_set_other_config(string session, string _pusb, Dictionary<string, string> _other_config)
        {
            var converters = new List<JsonConverter> {new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            Rpc("PUSB.set_other_config", new JArray(session, _pusb ?? "", _other_config == null ? new JObject() : JObject.FromObject(_other_config, serializer)), serializer);
        }

        public void pusb_add_to_other_config(string session, string _pusb, string _key, string _value)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("PUSB.add_to_other_config", new JArray(session, _pusb ?? "", _key ?? "", _value ?? ""), serializer);
        }

        public void pusb_remove_from_other_config(string session, string _pusb, string _key)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("PUSB.remove_from_other_config", new JArray(session, _pusb ?? "", _key ?? ""), serializer);
        }

        public void pusb_scan(string session, string _host)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Host>()};
            var serializer = CreateSerializer(converters);
            Rpc("PUSB.scan", new JArray(session, _host ?? ""), serializer);
        }

        public XenRef<Task> async_pusb_scan(string session, string _host)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>(), new XenRefConverter<Host>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.PUSB.scan", new JArray(session, _host ?? ""), serializer);
        }

        public void pusb_set_passthrough_enabled(string session, string _pusb, bool _value)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("PUSB.set_passthrough_enabled", new JArray(session, _pusb ?? "", _value), serializer);
        }

        public XenRef<Task> async_pusb_set_passthrough_enabled(string session, string _pusb, bool _value)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.PUSB.set_passthrough_enabled", new JArray(session, _pusb ?? "", _value), serializer);
        }

        public List<XenRef<PUSB>> pusb_get_all(string session)
        {
            var converters = new List<JsonConverter> {new XenRefListConverter<PUSB>()};
            var serializer = CreateSerializer(converters);
            return Rpc<List<XenRef<PUSB>>>("PUSB.get_all", new JArray(session), serializer);
        }

        public Dictionary<XenRef<PUSB>, PUSB> pusb_get_all_records(string session)
        {
            var converters = new List<JsonConverter> {new XenRefXenObjectMapConverter<PUSB>()};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<XenRef<PUSB>, PUSB>>("PUSB.get_all_records", new JArray(session), serializer);
        }

        public USB_group usb_group_get_record(string session, string _usb_group)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<USB_group>("USB_group.get_record", new JArray(session, _usb_group ?? ""), serializer);
        }

        public XenRef<USB_group> usb_group_get_by_uuid(string session, string _uuid)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<USB_group>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<USB_group>>("USB_group.get_by_uuid", new JArray(session, _uuid ?? ""), serializer);
        }

        public List<XenRef<USB_group>> usb_group_get_by_name_label(string session, string _label)
        {
            var converters = new List<JsonConverter> {new XenRefListConverter<USB_group>()};
            var serializer = CreateSerializer(converters);
            return Rpc<List<XenRef<USB_group>>>("USB_group.get_by_name_label", new JArray(session, _label ?? ""), serializer);
        }

        public string usb_group_get_uuid(string session, string _usb_group)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("USB_group.get_uuid", new JArray(session, _usb_group ?? ""), serializer);
        }

        public string usb_group_get_name_label(string session, string _usb_group)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("USB_group.get_name_label", new JArray(session, _usb_group ?? ""), serializer);
        }

        public string usb_group_get_name_description(string session, string _usb_group)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("USB_group.get_name_description", new JArray(session, _usb_group ?? ""), serializer);
        }

        public List<XenRef<PUSB>> usb_group_get_pusbs(string session, string _usb_group)
        {
            var converters = new List<JsonConverter> {new XenRefListConverter<PUSB>()};
            var serializer = CreateSerializer(converters);
            return Rpc<List<XenRef<PUSB>>>("USB_group.get_PUSBs", new JArray(session, _usb_group ?? ""), serializer);
        }

        public List<XenRef<VUSB>> usb_group_get_vusbs(string session, string _usb_group)
        {
            var converters = new List<JsonConverter> {new XenRefListConverter<VUSB>()};
            var serializer = CreateSerializer(converters);
            return Rpc<List<XenRef<VUSB>>>("USB_group.get_VUSBs", new JArray(session, _usb_group ?? ""), serializer);
        }

        public Dictionary<string, string> usb_group_get_other_config(string session, string _usb_group)
        {
            var converters = new List<JsonConverter> {new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<string, string>>("USB_group.get_other_config", new JArray(session, _usb_group ?? ""), serializer);
        }

        public void usb_group_set_name_label(string session, string _usb_group, string _label)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("USB_group.set_name_label", new JArray(session, _usb_group ?? "", _label ?? ""), serializer);
        }

        public void usb_group_set_name_description(string session, string _usb_group, string _description)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("USB_group.set_name_description", new JArray(session, _usb_group ?? "", _description ?? ""), serializer);
        }

        public void usb_group_set_other_config(string session, string _usb_group, Dictionary<string, string> _other_config)
        {
            var converters = new List<JsonConverter> {new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            Rpc("USB_group.set_other_config", new JArray(session, _usb_group ?? "", _other_config == null ? new JObject() : JObject.FromObject(_other_config, serializer)), serializer);
        }

        public void usb_group_add_to_other_config(string session, string _usb_group, string _key, string _value)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("USB_group.add_to_other_config", new JArray(session, _usb_group ?? "", _key ?? "", _value ?? ""), serializer);
        }

        public void usb_group_remove_from_other_config(string session, string _usb_group, string _key)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("USB_group.remove_from_other_config", new JArray(session, _usb_group ?? "", _key ?? ""), serializer);
        }

        public XenRef<USB_group> usb_group_create(string session, string _name_label, string _name_description, Dictionary<string, string> _other_config)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<USB_group>(), new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<USB_group>>("USB_group.create", new JArray(session, _name_label ?? "", _name_description ?? "", _other_config == null ? new JObject() : JObject.FromObject(_other_config, serializer)), serializer);
        }

        public XenRef<Task> async_usb_group_create(string session, string _name_label, string _name_description, Dictionary<string, string> _other_config)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>(), new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.USB_group.create", new JArray(session, _name_label ?? "", _name_description ?? "", _other_config == null ? new JObject() : JObject.FromObject(_other_config, serializer)), serializer);
        }

        public void usb_group_destroy(string session, string _usb_group)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("USB_group.destroy", new JArray(session, _usb_group ?? ""), serializer);
        }

        public XenRef<Task> async_usb_group_destroy(string session, string _usb_group)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.USB_group.destroy", new JArray(session, _usb_group ?? ""), serializer);
        }

        public List<XenRef<USB_group>> usb_group_get_all(string session)
        {
            var converters = new List<JsonConverter> {new XenRefListConverter<USB_group>()};
            var serializer = CreateSerializer(converters);
            return Rpc<List<XenRef<USB_group>>>("USB_group.get_all", new JArray(session), serializer);
        }

        public Dictionary<XenRef<USB_group>, USB_group> usb_group_get_all_records(string session)
        {
            var converters = new List<JsonConverter> {new XenRefXenObjectMapConverter<USB_group>()};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<XenRef<USB_group>, USB_group>>("USB_group.get_all_records", new JArray(session), serializer);
        }

        public VUSB vusb_get_record(string session, string _vusb)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<VUSB>("VUSB.get_record", new JArray(session, _vusb ?? ""), serializer);
        }

        public XenRef<VUSB> vusb_get_by_uuid(string session, string _uuid)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<VUSB>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<VUSB>>("VUSB.get_by_uuid", new JArray(session, _uuid ?? ""), serializer);
        }

        public string vusb_get_uuid(string session, string _vusb)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("VUSB.get_uuid", new JArray(session, _vusb ?? ""), serializer);
        }

        public List<vusb_operations> vusb_get_allowed_operations(string session, string _vusb)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<List<vusb_operations>>("VUSB.get_allowed_operations", new JArray(session, _vusb ?? ""), serializer);
        }

        public Dictionary<string, vusb_operations> vusb_get_current_operations(string session, string _vusb)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<string, vusb_operations>>("VUSB.get_current_operations", new JArray(session, _vusb ?? ""), serializer);
        }

        public XenRef<VM> vusb_get_vm(string session, string _vusb)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<VM>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<VM>>("VUSB.get_VM", new JArray(session, _vusb ?? ""), serializer);
        }

        public XenRef<USB_group> vusb_get_usb_group(string session, string _vusb)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<USB_group>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<USB_group>>("VUSB.get_USB_group", new JArray(session, _vusb ?? ""), serializer);
        }

        public Dictionary<string, string> vusb_get_other_config(string session, string _vusb)
        {
            var converters = new List<JsonConverter> {new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<string, string>>("VUSB.get_other_config", new JArray(session, _vusb ?? ""), serializer);
        }

        public bool vusb_get_currently_attached(string session, string _vusb)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<bool>("VUSB.get_currently_attached", new JArray(session, _vusb ?? ""), serializer);
        }

        public void vusb_set_other_config(string session, string _vusb, Dictionary<string, string> _other_config)
        {
            var converters = new List<JsonConverter> {new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            Rpc("VUSB.set_other_config", new JArray(session, _vusb ?? "", _other_config == null ? new JObject() : JObject.FromObject(_other_config, serializer)), serializer);
        }

        public void vusb_add_to_other_config(string session, string _vusb, string _key, string _value)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("VUSB.add_to_other_config", new JArray(session, _vusb ?? "", _key ?? "", _value ?? ""), serializer);
        }

        public void vusb_remove_from_other_config(string session, string _vusb, string _key)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("VUSB.remove_from_other_config", new JArray(session, _vusb ?? "", _key ?? ""), serializer);
        }

        public XenRef<VUSB> vusb_create(string session, string _vm, string _usb_group, Dictionary<string, string> _other_config)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<VUSB>(), new XenRefConverter<VM>(), new XenRefConverter<USB_group>(), new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<VUSB>>("VUSB.create", new JArray(session, _vm ?? "", _usb_group ?? "", _other_config == null ? new JObject() : JObject.FromObject(_other_config, serializer)), serializer);
        }

        public XenRef<Task> async_vusb_create(string session, string _vm, string _usb_group, Dictionary<string, string> _other_config)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>(), new XenRefConverter<VM>(), new XenRefConverter<USB_group>(), new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.VUSB.create", new JArray(session, _vm ?? "", _usb_group ?? "", _other_config == null ? new JObject() : JObject.FromObject(_other_config, serializer)), serializer);
        }

        public void vusb_unplug(string session, string _vusb)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("VUSB.unplug", new JArray(session, _vusb ?? ""), serializer);
        }

        public XenRef<Task> async_vusb_unplug(string session, string _vusb)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.VUSB.unplug", new JArray(session, _vusb ?? ""), serializer);
        }

        public void vusb_destroy(string session, string _vusb)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("VUSB.destroy", new JArray(session, _vusb ?? ""), serializer);
        }

        public XenRef<Task> async_vusb_destroy(string session, string _vusb)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.VUSB.destroy", new JArray(session, _vusb ?? ""), serializer);
        }

        public List<XenRef<VUSB>> vusb_get_all(string session)
        {
            var converters = new List<JsonConverter> {new XenRefListConverter<VUSB>()};
            var serializer = CreateSerializer(converters);
            return Rpc<List<XenRef<VUSB>>>("VUSB.get_all", new JArray(session), serializer);
        }

        public Dictionary<XenRef<VUSB>, VUSB> vusb_get_all_records(string session)
        {
            var converters = new List<JsonConverter> {new XenRefXenObjectMapConverter<VUSB>()};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<XenRef<VUSB>, VUSB>>("VUSB.get_all_records", new JArray(session), serializer);
        }

        public Cluster cluster_get_record(string session, string _cluster)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<Cluster>("Cluster.get_record", new JArray(session, _cluster ?? ""), serializer);
        }

        public XenRef<Cluster> cluster_get_by_uuid(string session, string _uuid)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Cluster>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Cluster>>("Cluster.get_by_uuid", new JArray(session, _uuid ?? ""), serializer);
        }

        public string cluster_get_uuid(string session, string _cluster)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("Cluster.get_uuid", new JArray(session, _cluster ?? ""), serializer);
        }

        public List<XenRef<Cluster_host>> cluster_get_cluster_hosts(string session, string _cluster)
        {
            var converters = new List<JsonConverter> {new XenRefListConverter<Cluster_host>()};
            var serializer = CreateSerializer(converters);
            return Rpc<List<XenRef<Cluster_host>>>("Cluster.get_cluster_hosts", new JArray(session, _cluster ?? ""), serializer);
        }

        public XenRef<Network> cluster_get_network(string session, string _cluster)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Network>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Network>>("Cluster.get_network", new JArray(session, _cluster ?? ""), serializer);
        }

        public string cluster_get_cluster_token(string session, string _cluster)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("Cluster.get_cluster_token", new JArray(session, _cluster ?? ""), serializer);
        }

        public string cluster_get_cluster_stack(string session, string _cluster)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("Cluster.get_cluster_stack", new JArray(session, _cluster ?? ""), serializer);
        }

        public List<cluster_operation> cluster_get_allowed_operations(string session, string _cluster)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<List<cluster_operation>>("Cluster.get_allowed_operations", new JArray(session, _cluster ?? ""), serializer);
        }

        public Dictionary<string, cluster_operation> cluster_get_current_operations(string session, string _cluster)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<string, cluster_operation>>("Cluster.get_current_operations", new JArray(session, _cluster ?? ""), serializer);
        }

        public bool cluster_get_pool_auto_join(string session, string _cluster)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<bool>("Cluster.get_pool_auto_join", new JArray(session, _cluster ?? ""), serializer);
        }

        public long cluster_get_token_timeout(string session, string _cluster)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<long>("Cluster.get_token_timeout", new JArray(session, _cluster ?? ""), serializer);
        }

        public long cluster_get_token_timeout_coefficient(string session, string _cluster)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<long>("Cluster.get_token_timeout_coefficient", new JArray(session, _cluster ?? ""), serializer);
        }

        public Dictionary<string, string> cluster_get_cluster_config(string session, string _cluster)
        {
            var converters = new List<JsonConverter> {new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<string, string>>("Cluster.get_cluster_config", new JArray(session, _cluster ?? ""), serializer);
        }

        public Dictionary<string, string> cluster_get_other_config(string session, string _cluster)
        {
            var converters = new List<JsonConverter> {new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<string, string>>("Cluster.get_other_config", new JArray(session, _cluster ?? ""), serializer);
        }

        public void cluster_set_other_config(string session, string _cluster, Dictionary<string, string> _other_config)
        {
            var converters = new List<JsonConverter> {new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            Rpc("Cluster.set_other_config", new JArray(session, _cluster ?? "", _other_config == null ? new JObject() : JObject.FromObject(_other_config, serializer)), serializer);
        }

        public void cluster_add_to_other_config(string session, string _cluster, string _key, string _value)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("Cluster.add_to_other_config", new JArray(session, _cluster ?? "", _key ?? "", _value ?? ""), serializer);
        }

        public void cluster_remove_from_other_config(string session, string _cluster, string _key)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("Cluster.remove_from_other_config", new JArray(session, _cluster ?? "", _key ?? ""), serializer);
        }

        public XenRef<Cluster> cluster_create(string session, string _network, string _cluster_stack, bool _pool_auto_join, double _token_timeout, double _token_timeout_coefficient)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Cluster>(), new XenRefConverter<Network>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Cluster>>("Cluster.create", new JArray(session, _network ?? "", _cluster_stack ?? "", _pool_auto_join, _token_timeout, _token_timeout_coefficient), serializer);
        }

        public XenRef<Task> async_cluster_create(string session, string _network, string _cluster_stack, bool _pool_auto_join, double _token_timeout, double _token_timeout_coefficient)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>(), new XenRefConverter<Network>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.Cluster.create", new JArray(session, _network ?? "", _cluster_stack ?? "", _pool_auto_join, _token_timeout, _token_timeout_coefficient), serializer);
        }

        public void cluster_destroy(string session, string _cluster)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("Cluster.destroy", new JArray(session, _cluster ?? ""), serializer);
        }

        public XenRef<Task> async_cluster_destroy(string session, string _cluster)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.Cluster.destroy", new JArray(session, _cluster ?? ""), serializer);
        }

        public XenRef<Cluster> cluster_pool_create(string session, string _network, string _cluster_stack, double _token_timeout, double _token_timeout_coefficient)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Cluster>(), new XenRefConverter<Network>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Cluster>>("Cluster.pool_create", new JArray(session, _network ?? "", _cluster_stack ?? "", _token_timeout, _token_timeout_coefficient), serializer);
        }

        public XenRef<Task> async_cluster_pool_create(string session, string _network, string _cluster_stack, double _token_timeout, double _token_timeout_coefficient)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>(), new XenRefConverter<Network>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.Cluster.pool_create", new JArray(session, _network ?? "", _cluster_stack ?? "", _token_timeout, _token_timeout_coefficient), serializer);
        }

        public void cluster_pool_force_destroy(string session, string _cluster)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("Cluster.pool_force_destroy", new JArray(session, _cluster ?? ""), serializer);
        }

        public XenRef<Task> async_cluster_pool_force_destroy(string session, string _cluster)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.Cluster.pool_force_destroy", new JArray(session, _cluster ?? ""), serializer);
        }

        public void cluster_pool_destroy(string session, string _cluster)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("Cluster.pool_destroy", new JArray(session, _cluster ?? ""), serializer);
        }

        public XenRef<Task> async_cluster_pool_destroy(string session, string _cluster)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.Cluster.pool_destroy", new JArray(session, _cluster ?? ""), serializer);
        }

        public void cluster_pool_resync(string session, string _cluster)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("Cluster.pool_resync", new JArray(session, _cluster ?? ""), serializer);
        }

        public XenRef<Task> async_cluster_pool_resync(string session, string _cluster)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.Cluster.pool_resync", new JArray(session, _cluster ?? ""), serializer);
        }

        public List<XenRef<Cluster>> cluster_get_all(string session)
        {
            var converters = new List<JsonConverter> {new XenRefListConverter<Cluster>()};
            var serializer = CreateSerializer(converters);
            return Rpc<List<XenRef<Cluster>>>("Cluster.get_all", new JArray(session), serializer);
        }

        public Dictionary<XenRef<Cluster>, Cluster> cluster_get_all_records(string session)
        {
            var converters = new List<JsonConverter> {new XenRefXenObjectMapConverter<Cluster>()};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<XenRef<Cluster>, Cluster>>("Cluster.get_all_records", new JArray(session), serializer);
        }

        public Cluster_host cluster_host_get_record(string session, string _cluster_host)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<Cluster_host>("Cluster_host.get_record", new JArray(session, _cluster_host ?? ""), serializer);
        }

        public XenRef<Cluster_host> cluster_host_get_by_uuid(string session, string _uuid)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Cluster_host>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Cluster_host>>("Cluster_host.get_by_uuid", new JArray(session, _uuid ?? ""), serializer);
        }

        public string cluster_host_get_uuid(string session, string _cluster_host)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<string>("Cluster_host.get_uuid", new JArray(session, _cluster_host ?? ""), serializer);
        }

        public XenRef<Cluster> cluster_host_get_cluster(string session, string _cluster_host)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Cluster>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Cluster>>("Cluster_host.get_cluster", new JArray(session, _cluster_host ?? ""), serializer);
        }

        public XenRef<Host> cluster_host_get_host(string session, string _cluster_host)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Host>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Host>>("Cluster_host.get_host", new JArray(session, _cluster_host ?? ""), serializer);
        }

        public bool cluster_host_get_enabled(string session, string _cluster_host)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<bool>("Cluster_host.get_enabled", new JArray(session, _cluster_host ?? ""), serializer);
        }

        public List<cluster_host_operation> cluster_host_get_allowed_operations(string session, string _cluster_host)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<List<cluster_host_operation>>("Cluster_host.get_allowed_operations", new JArray(session, _cluster_host ?? ""), serializer);
        }

        public Dictionary<string, cluster_host_operation> cluster_host_get_current_operations(string session, string _cluster_host)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<string, cluster_host_operation>>("Cluster_host.get_current_operations", new JArray(session, _cluster_host ?? ""), serializer);
        }

        public Dictionary<string, string> cluster_host_get_other_config(string session, string _cluster_host)
        {
            var converters = new List<JsonConverter> {new StringStringMapConverter()};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<string, string>>("Cluster_host.get_other_config", new JArray(session, _cluster_host ?? ""), serializer);
        }

        public XenRef<Cluster_host> cluster_host_create(string session, string _cluster, string _host)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Cluster_host>(), new XenRefConverter<Cluster>(), new XenRefConverter<Host>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Cluster_host>>("Cluster_host.create", new JArray(session, _cluster ?? "", _host ?? ""), serializer);
        }

        public XenRef<Task> async_cluster_host_create(string session, string _cluster, string _host)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>(), new XenRefConverter<Cluster>(), new XenRefConverter<Host>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.Cluster_host.create", new JArray(session, _cluster ?? "", _host ?? ""), serializer);
        }

        public void cluster_host_destroy(string session, string _cluster_host)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("Cluster_host.destroy", new JArray(session, _cluster_host ?? ""), serializer);
        }

        public XenRef<Task> async_cluster_host_destroy(string session, string _cluster_host)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.Cluster_host.destroy", new JArray(session, _cluster_host ?? ""), serializer);
        }

        public void cluster_host_enable(string session, string _cluster_host)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("Cluster_host.enable", new JArray(session, _cluster_host ?? ""), serializer);
        }

        public XenRef<Task> async_cluster_host_enable(string session, string _cluster_host)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.Cluster_host.enable", new JArray(session, _cluster_host ?? ""), serializer);
        }

        public void cluster_host_force_destroy(string session, string _cluster_host)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("Cluster_host.force_destroy", new JArray(session, _cluster_host ?? ""), serializer);
        }

        public XenRef<Task> async_cluster_host_force_destroy(string session, string _cluster_host)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.Cluster_host.force_destroy", new JArray(session, _cluster_host ?? ""), serializer);
        }

        public void cluster_host_disable(string session, string _cluster_host)
        {
            var converters = new List<JsonConverter> {};
            var serializer = CreateSerializer(converters);
            Rpc("Cluster_host.disable", new JArray(session, _cluster_host ?? ""), serializer);
        }

        public XenRef<Task> async_cluster_host_disable(string session, string _cluster_host)
        {
            var converters = new List<JsonConverter> {new XenRefConverter<Task>()};
            var serializer = CreateSerializer(converters);
            return Rpc<XenRef<Task>>("Async.Cluster_host.disable", new JArray(session, _cluster_host ?? ""), serializer);
        }

        public List<XenRef<Cluster_host>> cluster_host_get_all(string session)
        {
            var converters = new List<JsonConverter> {new XenRefListConverter<Cluster_host>()};
            var serializer = CreateSerializer(converters);
            return Rpc<List<XenRef<Cluster_host>>>("Cluster_host.get_all", new JArray(session), serializer);
        }

        public Dictionary<XenRef<Cluster_host>, Cluster_host> cluster_host_get_all_records(string session)
        {
            var converters = new List<JsonConverter> {new XenRefXenObjectMapConverter<Cluster_host>()};
            var serializer = CreateSerializer(converters);
            return Rpc<Dictionary<XenRef<Cluster_host>, Cluster_host>>("Cluster_host.get_all_records", new JArray(session), serializer);
        }

    }
}
