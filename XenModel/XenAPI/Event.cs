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
using System.Collections.Generic;
using Newtonsoft.Json;

namespace XenAPI
{
    [JsonConverter(typeof(XenEventConverter))]
    public partial class Event : XenObject<Event>
    {
        public Event()
        {
        }

        public override void UpdateFrom(Event update)
        {
            id = update.id;
        }

        public static Event get_record(Session session, string _event)
        {
            return session.JsonRpcClient.event_get_record(session.opaque_ref, _event);
        }

        public static string get_by_uuid(Session session, string _uuid)
        {
            return session.JsonRpcClient.event_get_by_uuid(session.opaque_ref, _uuid);
        }

        public static long get_id(Session session, string _event)
        {
            return session.JsonRpcClient.event_get_id(session.opaque_ref, _event);
        }

        public static void set_id(Session session, string _event, long _id)
        {
            session.JsonRpcClient.event_set_id(session.opaque_ref, _event, _id);
        }

        public static void register(Session session, string[] _classes)
        {
            session.JsonRpcClient.event_register(session.opaque_ref, _classes);
        }

        public static void unregister(Session session, string[] _classes)
        {
            session.JsonRpcClient.event_unregister(session.opaque_ref, _classes);
        }

        public static IEventCollection from(Session session, string[] _classes, string _token, double _timeout)
        {
            return session.JsonRpcClient.event_from(session.opaque_ref, _classes, _token, _timeout);
        }

        private long _id;
        public long id
        {
             get { return _id; }
             set
             {
                 if (value != _id)
                 {
                     _id = value;
                     NotifyPropertyChanged("id");
                 }
             }
        }

        public string timestamp;

        public string class_;

        public string operation;

        public string opaqueRef;

        public object snapshot;
    }


    public class EventBatch : IEventCollection
    {
        public Event[] events;
        public Dictionary<string, int> valid_ref_counts;
        public string token;
    }

    public interface IEventCollection
    {
    }
}
