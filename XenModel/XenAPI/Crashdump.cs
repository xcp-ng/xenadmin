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
    /// A VM crashdump
    /// First published in XenServer 4.0.
    /// </summary>
    public partial class Crashdump : XenObject<Crashdump>
    {
        #region Constructors

        public Crashdump()
        {
        }

        public Crashdump(string uuid,
            XenRef<VM> VM,
            XenRef<VDI> VDI,
            Dictionary<string, string> other_config)
        {
            this.uuid = uuid;
            this.VM = VM;
            this.VDI = VDI;
            this.other_config = other_config;
        }

        /// <summary>
        /// Creates a new Crashdump from a Hashtable.
        /// Note that the fields not contained in the Hashtable
        /// will be created with their default values.
        /// </summary>
        /// <param name="table"></param>
        public Crashdump(Hashtable table)
            : this()
        {
            UpdateFrom(table);
        }

        #endregion

        /// <summary>
        /// Updates each field of this instance with the value of
        /// the corresponding field of a given Crashdump.
        /// </summary>
        public override void UpdateFrom(Crashdump record)
        {
            uuid = record.uuid;
            VM = record.VM;
            VDI = record.VDI;
            other_config = record.other_config;
        }

        /// <summary>
        /// Given a Hashtable with field-value pairs, it updates the fields of this Crashdump
        /// with the values listed in the Hashtable. Note that only the fields contained
        /// in the Hashtable will be updated and the rest will remain the same.
        /// </summary>
        /// <param name="table"></param>
        public void UpdateFrom(Hashtable table)
        {
            if (table.ContainsKey("uuid"))
                uuid = Marshalling.ParseString(table, "uuid");
            if (table.ContainsKey("VM"))
                VM = Marshalling.ParseRef<VM>(table, "VM");
            if (table.ContainsKey("VDI"))
                VDI = Marshalling.ParseRef<VDI>(table, "VDI");
            if (table.ContainsKey("other_config"))
                other_config = Maps.ToDictionary_string_string(Marshalling.ParseHashTable(table, "other_config"));
        }

        public bool DeepEquals(Crashdump other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;

            return Helper.AreEqual2(_uuid, other._uuid) &&
                Helper.AreEqual2(_VM, other._VM) &&
                Helper.AreEqual2(_VDI, other._VDI) &&
                Helper.AreEqual2(_other_config, other._other_config);
        }


        /// <summary>
        /// Get a record containing the current state of the given crashdump.
        /// First published in XenServer 4.0.
        /// Deprecated since XenServer 7.3.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_crashdump">The opaque_ref of the given crashdump</param>
        [Deprecated("XenServer 7.3")]
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static Crashdump get_record(Session session, string _crashdump)
        {
            return session.JsonRpcClient.crashdump_get_record(session.opaque_ref, _crashdump);
        }

        /// <summary>
        /// Get a reference to the crashdump instance with the specified UUID.
        /// First published in XenServer 4.0.
        /// Deprecated since XenServer 7.3.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_uuid">UUID of object to return</param>
        [Deprecated("XenServer 7.3")]
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static XenRef<Crashdump> get_by_uuid(Session session, string _uuid)
        {
            return session.JsonRpcClient.crashdump_get_by_uuid(session.opaque_ref, _uuid);
        }

        /// <summary>
        /// Get the uuid field of the given crashdump.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_crashdump">The opaque_ref of the given crashdump</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_uuid(Session session, string _crashdump)
        {
            return session.JsonRpcClient.crashdump_get_uuid(session.opaque_ref, _crashdump);
        }

        /// <summary>
        /// Get the VM field of the given crashdump.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_crashdump">The opaque_ref of the given crashdump</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static XenRef<VM> get_VM(Session session, string _crashdump)
        {
            return session.JsonRpcClient.crashdump_get_vm(session.opaque_ref, _crashdump);
        }

        /// <summary>
        /// Get the VDI field of the given crashdump.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_crashdump">The opaque_ref of the given crashdump</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static XenRef<VDI> get_VDI(Session session, string _crashdump)
        {
            return session.JsonRpcClient.crashdump_get_vdi(session.opaque_ref, _crashdump);
        }

        /// <summary>
        /// Get the other_config field of the given crashdump.
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_crashdump">The opaque_ref of the given crashdump</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static Dictionary<string, string> get_other_config(Session session, string _crashdump)
        {
            return session.JsonRpcClient.crashdump_get_other_config(session.opaque_ref, _crashdump);
        }

        /// <summary>
        /// Set the other_config field of the given crashdump.
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_crashdump">The opaque_ref of the given crashdump</param>
        /// <param name="_other_config">New value to set</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void set_other_config(Session session, string _crashdump, Dictionary<string, string> _other_config)
        {
            session.JsonRpcClient.crashdump_set_other_config(session.opaque_ref, _crashdump, _other_config);
        }

        /// <summary>
        /// Add the given key-value pair to the other_config field of the given crashdump.
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_crashdump">The opaque_ref of the given crashdump</param>
        /// <param name="_key">Key to add</param>
        /// <param name="_value">Value to add</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void add_to_other_config(Session session, string _crashdump, string _key, string _value)
        {
            session.JsonRpcClient.crashdump_add_to_other_config(session.opaque_ref, _crashdump, _key, _value);
        }

        /// <summary>
        /// Remove the given key and its corresponding value from the other_config field of the given crashdump.  If the key is not in that Map, then do nothing.
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_crashdump">The opaque_ref of the given crashdump</param>
        /// <param name="_key">Key to remove</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void remove_from_other_config(Session session, string _crashdump, string _key)
        {
            session.JsonRpcClient.crashdump_remove_from_other_config(session.opaque_ref, _crashdump, _key);
        }

        /// <summary>
        /// Destroy the specified crashdump
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_crashdump">The opaque_ref of the given crashdump</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void destroy(Session session, string _crashdump)
        {
            session.JsonRpcClient.crashdump_destroy(session.opaque_ref, _crashdump);
        }

        /// <summary>
        /// Destroy the specified crashdump
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_crashdump">The opaque_ref of the given crashdump</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Task> async_destroy(Session session, string _crashdump)
        {
          return session.JsonRpcClient.async_crashdump_destroy(session.opaque_ref, _crashdump);
        }

        /// <summary>
        /// Return a list of all the crashdumps known to the system.
        /// First published in XenServer 4.0.
        /// Deprecated since XenServer 7.3.
        /// </summary>
        /// <param name="session">The session</param>
        [Deprecated("XenServer 7.3")]
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static List<XenRef<Crashdump>> get_all(Session session)
        {
            return session.JsonRpcClient.crashdump_get_all(session.opaque_ref);
        }

        /// <summary>
        /// Return a map of crashdump references to crashdump records for all crashdumps known to the system.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static Dictionary<XenRef<Crashdump>, Crashdump> get_all_records(Session session)
        {
            return session.JsonRpcClient.crashdump_get_all_records(session.opaque_ref);
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
        /// the virtual machine
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
        /// the virtual disk
        /// </summary>
        [JsonConverter(typeof(XenRefConverter<VDI>))]
        public virtual XenRef<VDI> VDI
        {
            get { return _VDI; }
            set
            {
                if (!Helper.AreEqual(value, _VDI))
                {
                    _VDI = value;
                    NotifyPropertyChanged("VDI");
                }
            }
        }
        private XenRef<VDI> _VDI = new XenRef<VDI>(Helper.NullOpaqueRef);

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
    }
}
