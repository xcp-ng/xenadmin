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
    /// A console
    /// First published in XenServer 4.0.
    /// </summary>
    public partial class Console : XenObject<Console>
    {
        #region Constructors

        public Console()
        {
        }

        public Console(string uuid,
            console_protocol protocol,
            string location,
            XenRef<VM> VM,
            Dictionary<string, string> other_config)
        {
            this.uuid = uuid;
            this.protocol = protocol;
            this.location = location;
            this.VM = VM;
            this.other_config = other_config;
        }

        /// <summary>
        /// Creates a new Console from a Hashtable.
        /// Note that the fields not contained in the Hashtable
        /// will be created with their default values.
        /// </summary>
        /// <param name="table"></param>
        public Console(Hashtable table)
            : this()
        {
            UpdateFrom(table);
        }

        #endregion

        /// <summary>
        /// Updates each field of this instance with the value of
        /// the corresponding field of a given Console.
        /// </summary>
        public override void UpdateFrom(Console record)
        {
            uuid = record.uuid;
            protocol = record.protocol;
            location = record.location;
            VM = record.VM;
            other_config = record.other_config;
        }

        /// <summary>
        /// Given a Hashtable with field-value pairs, it updates the fields of this Console
        /// with the values listed in the Hashtable. Note that only the fields contained
        /// in the Hashtable will be updated and the rest will remain the same.
        /// </summary>
        /// <param name="table"></param>
        public void UpdateFrom(Hashtable table)
        {
            if (table.ContainsKey("uuid"))
                uuid = Marshalling.ParseString(table, "uuid");
            if (table.ContainsKey("protocol"))
                protocol = (console_protocol)Helper.EnumParseDefault(typeof(console_protocol), Marshalling.ParseString(table, "protocol"));
            if (table.ContainsKey("location"))
                location = Marshalling.ParseString(table, "location");
            if (table.ContainsKey("VM"))
                VM = Marshalling.ParseRef<VM>(table, "VM");
            if (table.ContainsKey("other_config"))
                other_config = Maps.ToDictionary_string_string(Marshalling.ParseHashTable(table, "other_config"));
        }

        public bool DeepEquals(Console other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;

            return Helper.AreEqual2(_uuid, other._uuid) &&
                Helper.AreEqual2(_protocol, other._protocol) &&
                Helper.AreEqual2(_location, other._location) &&
                Helper.AreEqual2(_VM, other._VM) &&
                Helper.AreEqual2(_other_config, other._other_config);
        }


        /// <summary>
        /// Get a record containing the current state of the given console.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_console">The opaque_ref of the given console</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static Console get_record(Session session, string _console)
        {
            return session.JsonRpcClient.console_get_record(session.opaque_ref, _console);
        }

        /// <summary>
        /// Get a reference to the console instance with the specified UUID.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_uuid">UUID of object to return</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static XenRef<Console> get_by_uuid(Session session, string _uuid)
        {
            return session.JsonRpcClient.console_get_by_uuid(session.opaque_ref, _uuid);
        }

        /// <summary>
        /// Create a new console instance, and return its handle.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_record">All constructor arguments</param>
        /// <remarks>
        /// Minimum allowed role: vm-admin
        /// </remarks>
        public static XenRef<Console> create(Session session, Console _record)
        {
            return session.JsonRpcClient.console_create(session.opaque_ref, _record);
        }

        /// <summary>
        /// Create a new console instance, and return its handle.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_record">All constructor arguments</param>
        /// <remarks>
        /// Minimum allowed role: vm-admin
        /// </remarks>
        public static XenRef<Task> async_create(Session session, Console _record)
        {
          return session.JsonRpcClient.async_console_create(session.opaque_ref, _record);
        }

        /// <summary>
        /// Destroy the specified console instance.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_console">The opaque_ref of the given console</param>
        /// <remarks>
        /// Minimum allowed role: vm-admin
        /// </remarks>
        public static void destroy(Session session, string _console)
        {
            session.JsonRpcClient.console_destroy(session.opaque_ref, _console);
        }

        /// <summary>
        /// Destroy the specified console instance.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_console">The opaque_ref of the given console</param>
        /// <remarks>
        /// Minimum allowed role: vm-admin
        /// </remarks>
        public static XenRef<Task> async_destroy(Session session, string _console)
        {
          return session.JsonRpcClient.async_console_destroy(session.opaque_ref, _console);
        }

        /// <summary>
        /// Get the uuid field of the given console.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_console">The opaque_ref of the given console</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_uuid(Session session, string _console)
        {
            return session.JsonRpcClient.console_get_uuid(session.opaque_ref, _console);
        }

        /// <summary>
        /// Get the protocol field of the given console.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_console">The opaque_ref of the given console</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static console_protocol get_protocol(Session session, string _console)
        {
            return session.JsonRpcClient.console_get_protocol(session.opaque_ref, _console);
        }

        /// <summary>
        /// Get the location field of the given console.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_console">The opaque_ref of the given console</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_location(Session session, string _console)
        {
            return session.JsonRpcClient.console_get_location(session.opaque_ref, _console);
        }

        /// <summary>
        /// Get the VM field of the given console.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_console">The opaque_ref of the given console</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static XenRef<VM> get_VM(Session session, string _console)
        {
            return session.JsonRpcClient.console_get_vm(session.opaque_ref, _console);
        }

        /// <summary>
        /// Get the other_config field of the given console.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_console">The opaque_ref of the given console</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static Dictionary<string, string> get_other_config(Session session, string _console)
        {
            return session.JsonRpcClient.console_get_other_config(session.opaque_ref, _console);
        }

        /// <summary>
        /// Set the other_config field of the given console.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_console">The opaque_ref of the given console</param>
        /// <param name="_other_config">New value to set</param>
        /// <remarks>
        /// Minimum allowed role: vm-admin
        /// </remarks>
        public static void set_other_config(Session session, string _console, Dictionary<string, string> _other_config)
        {
            session.JsonRpcClient.console_set_other_config(session.opaque_ref, _console, _other_config);
        }

        /// <summary>
        /// Add the given key-value pair to the other_config field of the given console.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_console">The opaque_ref of the given console</param>
        /// <param name="_key">Key to add</param>
        /// <param name="_value">Value to add</param>
        /// <remarks>
        /// Minimum allowed role: vm-admin
        /// </remarks>
        public static void add_to_other_config(Session session, string _console, string _key, string _value)
        {
            session.JsonRpcClient.console_add_to_other_config(session.opaque_ref, _console, _key, _value);
        }

        /// <summary>
        /// Remove the given key and its corresponding value from the other_config field of the given console.  If the key is not in that Map, then do nothing.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_console">The opaque_ref of the given console</param>
        /// <param name="_key">Key to remove</param>
        /// <remarks>
        /// Minimum allowed role: vm-admin
        /// </remarks>
        public static void remove_from_other_config(Session session, string _console, string _key)
        {
            session.JsonRpcClient.console_remove_from_other_config(session.opaque_ref, _console, _key);
        }

        /// <summary>
        /// Return a list of all the consoles known to the system.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static List<XenRef<Console>> get_all(Session session)
        {
            return session.JsonRpcClient.console_get_all(session.opaque_ref);
        }

        /// <summary>
        /// Return a map of console references to console records for all consoles known to the system.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static Dictionary<XenRef<Console>, Console> get_all_records(Session session)
        {
            return session.JsonRpcClient.console_get_all_records(session.opaque_ref);
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
        /// the protocol used by this console
        /// </summary>
        [JsonConverter(typeof(console_protocolConverter))]
        public virtual console_protocol protocol
        {
            get { return _protocol; }
            set
            {
                if (!Helper.AreEqual(value, _protocol))
                {
                    _protocol = value;
                    NotifyPropertyChanged("protocol");
                }
            }
        }
        private console_protocol _protocol;

        /// <summary>
        /// URI for the console service
        /// </summary>
        public virtual string location
        {
            get { return _location; }
            set
            {
                if (!Helper.AreEqual(value, _location))
                {
                    _location = value;
                    NotifyPropertyChanged("location");
                }
            }
        }
        private string _location = "";

        /// <summary>
        /// VM to which this console is attached
        /// </summary>
        [JsonConverter(typeof(XenRefConverter<VM>))]
        public virtual XenRef<VM> VM
        {
            get { return _VM; }
            set
            {
                if (!Helper.AreEqual(value, _VM))
                {
                    _VM = value;
                    NotifyPropertyChanged("VM");
                }
            }
        }
        private XenRef<VM> _VM = new XenRef<VM>(Helper.NullOpaqueRef);

        /// <summary>
        /// additional configuration
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
    }
}
