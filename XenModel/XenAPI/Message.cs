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
    /// An message for the attention of the administrator
    /// First published in XenServer 5.0.
    /// </summary>
    public partial class Message : XenObject<Message>
    {
        #region Constructors

        public Message()
        {
        }

        public Message(string uuid,
            string name,
            long priority,
            cls cls,
            string obj_uuid,
            DateTime timestamp,
            string body)
        {
            this.uuid = uuid;
            this.name = name;
            this.priority = priority;
            this.cls = cls;
            this.obj_uuid = obj_uuid;
            this.timestamp = timestamp;
            this.body = body;
        }

        /// <summary>
        /// Creates a new Message from a Hashtable.
        /// Note that the fields not contained in the Hashtable
        /// will be created with their default values.
        /// </summary>
        /// <param name="table"></param>
        public Message(Hashtable table)
            : this()
        {
            UpdateFrom(table);
        }

        #endregion

        /// <summary>
        /// Updates each field of this instance with the value of
        /// the corresponding field of a given Message.
        /// </summary>
        public override void UpdateFrom(Message record)
        {
            uuid = record.uuid;
            name = record.name;
            priority = record.priority;
            cls = record.cls;
            obj_uuid = record.obj_uuid;
            timestamp = record.timestamp;
            body = record.body;
        }

        /// <summary>
        /// Given a Hashtable with field-value pairs, it updates the fields of this Message
        /// with the values listed in the Hashtable. Note that only the fields contained
        /// in the Hashtable will be updated and the rest will remain the same.
        /// </summary>
        /// <param name="table"></param>
        public void UpdateFrom(Hashtable table)
        {
            if (table.ContainsKey("uuid"))
                uuid = Marshalling.ParseString(table, "uuid");
            if (table.ContainsKey("name"))
                name = Marshalling.ParseString(table, "name");
            if (table.ContainsKey("priority"))
                priority = Marshalling.ParseLong(table, "priority");
            if (table.ContainsKey("cls"))
                cls = (cls)Helper.EnumParseDefault(typeof(cls), Marshalling.ParseString(table, "cls"));
            if (table.ContainsKey("obj_uuid"))
                obj_uuid = Marshalling.ParseString(table, "obj_uuid");
            if (table.ContainsKey("timestamp"))
                timestamp = Marshalling.ParseDateTime(table, "timestamp");
            if (table.ContainsKey("body"))
                body = Marshalling.ParseString(table, "body");
        }

        public bool DeepEquals(Message other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;

            return Helper.AreEqual2(_uuid, other._uuid) &&
                Helper.AreEqual2(_name, other._name) &&
                Helper.AreEqual2(_priority, other._priority) &&
                Helper.AreEqual2(_cls, other._cls) &&
                Helper.AreEqual2(_obj_uuid, other._obj_uuid) &&
                Helper.AreEqual2(_timestamp, other._timestamp) &&
                Helper.AreEqual2(_body, other._body);
        }


        /// <summary>
        /// 
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_name">The name of the message</param>
        /// <param name="_priority">The priority of the message</param>
        /// <param name="_cls">The class of object this message is associated with</param>
        /// <param name="_obj_uuid">The uuid of the object this message is associated with</param>
        /// <param name="_body">The body of the message</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Message> create(Session session, string _name, long _priority, cls _cls, string _obj_uuid, string _body)
        {
            return session.JsonRpcClient.message_create(session.opaque_ref, _name, _priority, _cls, _obj_uuid, _body);
        }

        /// <summary>
        /// 
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_message">The opaque_ref of the given message</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void destroy(Session session, string _message)
        {
            session.JsonRpcClient.message_destroy(session.opaque_ref, _message);
        }

        /// <summary>
        /// 
        /// Experimental. First published in 22.19.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_messages">Messages to destroy</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void destroy_many(Session session, List<XenRef<Message>> _messages)
        {
            session.JsonRpcClient.message_destroy_many(session.opaque_ref, _messages);
        }

        /// <summary>
        /// 
        /// Experimental. First published in 22.19.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_messages">Messages to destroy</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Task> async_destroy_many(Session session, List<XenRef<Message>> _messages)
        {
          return session.JsonRpcClient.async_message_destroy_many(session.opaque_ref, _messages);
        }

        /// <summary>
        /// 
        /// Experimental. First published in 26.6.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_filters">Optional filters identifying messages to destroy: before (RFC3339 DateTime, destroy messages dated before this timestamp), after (RFC3339 DateTime, destroy messages dated after this timestamp), and priority (int, only destroy messages with this priority). All provided conditions must be met (logical AND). If no filters are provided, all messages are destroyed. If no timezone is specified in a timestamp, UTC is assumed.</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void destroy_all(Session session, Dictionary<string, string> _filters)
        {
            session.JsonRpcClient.message_destroy_all(session.opaque_ref, _filters);
        }

        /// <summary>
        /// 
        /// Experimental. First published in 26.6.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_filters">Optional filters identifying messages to destroy: before (RFC3339 DateTime, destroy messages dated before this timestamp), after (RFC3339 DateTime, destroy messages dated after this timestamp), and priority (int, only destroy messages with this priority). All provided conditions must be met (logical AND). If no filters are provided, all messages are destroyed. If no timezone is specified in a timestamp, UTC is assumed.</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Task> async_destroy_all(Session session, Dictionary<string, string> _filters)
        {
          return session.JsonRpcClient.async_message_destroy_all(session.opaque_ref, _filters);
        }

        /// <summary>
        /// 
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_cls">The class of object</param>
        /// <param name="_obj_uuid">The uuid of the object</param>
        /// <param name="_since">The cutoff time. When the timezone is missing, UTC is assumed</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static Dictionary<XenRef<Message>, Message> get(Session session, cls _cls, string _obj_uuid, DateTime _since)
        {
            return session.JsonRpcClient.message_get(session.opaque_ref, _cls, _obj_uuid, _since);
        }

        /// <summary>
        /// 
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static List<XenRef<Message>> get_all(Session session)
        {
            return session.JsonRpcClient.message_get_all(session.opaque_ref);
        }

        /// <summary>
        /// 
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_since">The cutoff time. When the timezone is missing, UTC is assumed</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static Dictionary<XenRef<Message>, Message> get_since(Session session, DateTime _since)
        {
            return session.JsonRpcClient.message_get_since(session.opaque_ref, _since);
        }

        /// <summary>
        /// 
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_message">The opaque_ref of the given message</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static Message get_record(Session session, string _message)
        {
            return session.JsonRpcClient.message_get_record(session.opaque_ref, _message);
        }

        /// <summary>
        /// 
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_uuid">The uuid of the message</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static XenRef<Message> get_by_uuid(Session session, string _uuid)
        {
            return session.JsonRpcClient.message_get_by_uuid(session.opaque_ref, _uuid);
        }

        /// <summary>
        /// 
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static Dictionary<XenRef<Message>, Message> get_all_records(Session session)
        {
            return session.JsonRpcClient.message_get_all_records(session.opaque_ref);
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
        /// The name of the message
        /// </summary>
        public virtual string name
        {
            get { return _name; }
            set
            {
                if (!Helper.AreEqual(value, _name))
                {
                    _name = value;
                    NotifyPropertyChanged("name");
                }
            }
        }
        private string _name = "";

        /// <summary>
        /// The message priority, 0 being low priority
        /// </summary>
        public virtual long priority
        {
            get { return _priority; }
            set
            {
                if (!Helper.AreEqual(value, _priority))
                {
                    _priority = value;
                    NotifyPropertyChanged("priority");
                }
            }
        }
        private long _priority;

        /// <summary>
        /// The class of the object this message is associated with
        /// </summary>
        [JsonConverter(typeof(clsConverter))]
        public virtual cls cls
        {
            get { return _cls; }
            set
            {
                if (!Helper.AreEqual(value, _cls))
                {
                    _cls = value;
                    NotifyPropertyChanged("cls");
                }
            }
        }
        private cls _cls;

        /// <summary>
        /// The uuid of the object this message is associated with
        /// </summary>
        public virtual string obj_uuid
        {
            get { return _obj_uuid; }
            set
            {
                if (!Helper.AreEqual(value, _obj_uuid))
                {
                    _obj_uuid = value;
                    NotifyPropertyChanged("obj_uuid");
                }
            }
        }
        private string _obj_uuid = "";

        /// <summary>
        /// The time at which the message was created
        /// </summary>
        [JsonConverter(typeof(XenDateTimeConverter))]
        public virtual DateTime timestamp
        {
            get { return _timestamp; }
            set
            {
                if (!Helper.AreEqual(value, _timestamp))
                {
                    _timestamp = value;
                    NotifyPropertyChanged("timestamp");
                }
            }
        }
        private DateTime _timestamp;

        /// <summary>
        /// The body of the message
        /// </summary>
        public virtual string body
        {
            get { return _body; }
            set
            {
                if (!Helper.AreEqual(value, _body))
                {
                    _body = value;
                    NotifyPropertyChanged("body");
                }
            }
        }
        private string _body = "";
    }
}
