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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
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

        /// <summary>
        /// Creates a new Message from a Proxy_Message.
        /// </summary>
        /// <param name="proxy"></param>
        public Message(Proxy_Message proxy)
        {
            UpdateFrom(proxy);
        }

        #endregion

        /// <summary>
        /// Updates each field of this instance with the value of
        /// the corresponding field of a given Message.
        /// </summary>
        public override void UpdateFrom(Message update)
        {
            uuid = update.uuid;
            name = update.name;
            priority = update.priority;
            cls = update.cls;
            obj_uuid = update.obj_uuid;
            timestamp = update.timestamp;
            body = update.body;
        }

        internal void UpdateFrom(Proxy_Message proxy)
        {
            uuid = proxy.uuid == null ? null : proxy.uuid;
            name = proxy.name == null ? null : proxy.name;
            priority = proxy.priority == null ? 0 : long.Parse(proxy.priority);
            cls = proxy.cls == null ? (cls) 0 : (cls)Helper.EnumParseDefault(typeof(cls), (string)proxy.cls);
            obj_uuid = proxy.obj_uuid == null ? null : proxy.obj_uuid;
            timestamp = proxy.timestamp;
            body = proxy.body == null ? null : proxy.body;
        }

        public Proxy_Message ToProxy()
        {
            Proxy_Message result_ = new Proxy_Message();
            result_.uuid = uuid ?? "";
            result_.name = name ?? "";
            result_.priority = priority.ToString();
            result_.cls = cls_helper.ToString(cls);
            result_.obj_uuid = obj_uuid ?? "";
            result_.timestamp = timestamp;
            result_.body = body ?? "";
            return result_;
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

            return Helper.AreEqual2(this._uuid, other._uuid) &&
                Helper.AreEqual2(this._name, other._name) &&
                Helper.AreEqual2(this._priority, other._priority) &&
                Helper.AreEqual2(this._cls, other._cls) &&
                Helper.AreEqual2(this._obj_uuid, other._obj_uuid) &&
                Helper.AreEqual2(this._timestamp, other._timestamp) &&
                Helper.AreEqual2(this._body, other._body);
        }

        internal static List<Message> ProxyArrayToObjectList(Proxy_Message[] input)
        {
            var result = new List<Message>();
            foreach (var item in input)
                result.Add(new Message(item));

            return result;
        }

        public override string SaveChanges(Session session, string opaqueRef, Message server)
        {
            if (opaqueRef == null)
            {
                System.Diagnostics.Debug.Assert(false, "Cannot create instances of this type on the server");
                return "";
            }
            else
            {
              throw new InvalidOperationException("This type has no read/write properties");
            }
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
        public static XenRef<Message> create(Session session, string _name, long _priority, cls _cls, string _obj_uuid, string _body)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.message_create(session.opaque_ref, _name, _priority, _cls, _obj_uuid, _body);
            else
                return XenRef<Message>.Create(session.XmlRpcProxy.message_create(session.opaque_ref, _name ?? "", _priority.ToString(), cls_helper.ToString(_cls), _obj_uuid ?? "", _body ?? "").parse());
        }

        /// <summary>
        /// 
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_message">The opaque_ref of the given message</param>
        public static void destroy(Session session, string _message)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.message_destroy(session.opaque_ref, _message);
            else
                session.XmlRpcProxy.message_destroy(session.opaque_ref, _message ?? "").parse();
        }

        /// <summary>
        /// 
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_cls">The class of object</param>
        /// <param name="_obj_uuid">The uuid of the object</param>
        /// <param name="_since">The cutoff time</param>
        public static Dictionary<XenRef<Message>, Message> get(Session session, cls _cls, string _obj_uuid, DateTime _since)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.message_get(session.opaque_ref, _cls, _obj_uuid, _since);
            else
                return XenRef<Message>.Create<Proxy_Message>(session.XmlRpcProxy.message_get(session.opaque_ref, cls_helper.ToString(_cls), _obj_uuid ?? "", _since).parse());
        }

        /// <summary>
        /// 
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        public static List<XenRef<Message>> get_all(Session session)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.message_get_all(session.opaque_ref);
            else
                return XenRef<Message>.Create(session.XmlRpcProxy.message_get_all(session.opaque_ref).parse());
        }

        /// <summary>
        /// 
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_since">The cutoff time</param>
        public static Dictionary<XenRef<Message>, Message> get_since(Session session, DateTime _since)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.message_get_since(session.opaque_ref, _since);
            else
                return XenRef<Message>.Create<Proxy_Message>(session.XmlRpcProxy.message_get_since(session.opaque_ref, _since).parse());
        }

        /// <summary>
        /// 
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_message">The opaque_ref of the given message</param>
        public static Message get_record(Session session, string _message)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.message_get_record(session.opaque_ref, _message);
            else
                return new Message(session.XmlRpcProxy.message_get_record(session.opaque_ref, _message ?? "").parse());
        }

        /// <summary>
        /// 
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_uuid">The uuid of the message</param>
        public static XenRef<Message> get_by_uuid(Session session, string _uuid)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.message_get_by_uuid(session.opaque_ref, _uuid);
            else
                return XenRef<Message>.Create(session.XmlRpcProxy.message_get_by_uuid(session.opaque_ref, _uuid ?? "").parse());
        }

        /// <summary>
        /// 
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        public static Dictionary<XenRef<Message>, Message> get_all_records(Session session)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.message_get_all_records(session.opaque_ref);
            else
                return XenRef<Message>.Create<Proxy_Message>(session.XmlRpcProxy.message_get_all_records(session.opaque_ref).parse());
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
