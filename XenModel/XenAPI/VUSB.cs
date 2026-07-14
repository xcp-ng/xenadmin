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
    /// Describes the vusb device
    /// First published in XenServer 7.3.
    /// </summary>
    public partial class VUSB : XenObject<VUSB>
    {
        #region Constructors

        public VUSB()
        {
        }

        public VUSB(string uuid,
            List<vusb_operations> allowed_operations,
            Dictionary<string, vusb_operations> current_operations,
            XenRef<VM> VM,
            XenRef<USB_group> USB_group,
            Dictionary<string, string> other_config,
            bool currently_attached)
        {
            this.uuid = uuid;
            this.allowed_operations = allowed_operations;
            this.current_operations = current_operations;
            this.VM = VM;
            this.USB_group = USB_group;
            this.other_config = other_config;
            this.currently_attached = currently_attached;
        }

        /// <summary>
        /// Creates a new VUSB from a Hashtable.
        /// Note that the fields not contained in the Hashtable
        /// will be created with their default values.
        /// </summary>
        /// <param name="table"></param>
        public VUSB(Hashtable table)
            : this()
        {
            UpdateFrom(table);
        }

        #endregion

        /// <summary>
        /// Updates each field of this instance with the value of
        /// the corresponding field of a given VUSB.
        /// </summary>
        public override void UpdateFrom(VUSB record)
        {
            uuid = record.uuid;
            allowed_operations = record.allowed_operations;
            current_operations = record.current_operations;
            VM = record.VM;
            USB_group = record.USB_group;
            other_config = record.other_config;
            currently_attached = record.currently_attached;
        }

        /// <summary>
        /// Given a Hashtable with field-value pairs, it updates the fields of this VUSB
        /// with the values listed in the Hashtable. Note that only the fields contained
        /// in the Hashtable will be updated and the rest will remain the same.
        /// </summary>
        /// <param name="table"></param>
        public void UpdateFrom(Hashtable table)
        {
            if (table.ContainsKey("uuid"))
                uuid = Marshalling.ParseString(table, "uuid");
            if (table.ContainsKey("allowed_operations"))
                allowed_operations = Helper.StringArrayToEnumList<vusb_operations>(Marshalling.ParseStringArray(table, "allowed_operations"));
            if (table.ContainsKey("current_operations"))
                current_operations = Maps.ToDictionary_string_vusb_operations(Marshalling.ParseHashTable(table, "current_operations"));
            if (table.ContainsKey("VM"))
                VM = Marshalling.ParseRef<VM>(table, "VM");
            if (table.ContainsKey("USB_group"))
                USB_group = Marshalling.ParseRef<USB_group>(table, "USB_group");
            if (table.ContainsKey("other_config"))
                other_config = Maps.ToDictionary_string_string(Marshalling.ParseHashTable(table, "other_config"));
            if (table.ContainsKey("currently_attached"))
                currently_attached = Marshalling.ParseBool(table, "currently_attached");
        }

        public bool DeepEquals(VUSB other, bool ignoreCurrentOperations)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;

            if (!ignoreCurrentOperations && !Helper.AreEqual2(current_operations, other.current_operations))
                return false;

            return Helper.AreEqual2(_uuid, other._uuid) &&
                Helper.AreEqual2(_allowed_operations, other._allowed_operations) &&
                Helper.AreEqual2(_VM, other._VM) &&
                Helper.AreEqual2(_USB_group, other._USB_group) &&
                Helper.AreEqual2(_other_config, other._other_config) &&
                Helper.AreEqual2(_currently_attached, other._currently_attached);
        }


        /// <summary>
        /// Get a record containing the current state of the given VUSB.
        /// First published in XenServer 7.3.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vusb">The opaque_ref of the given vusb</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static VUSB get_record(Session session, string _vusb)
        {
            return session.JsonRpcClient.vusb_get_record(session.opaque_ref, _vusb);
        }

        /// <summary>
        /// Get a reference to the VUSB instance with the specified UUID.
        /// First published in XenServer 7.3.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_uuid">UUID of object to return</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static XenRef<VUSB> get_by_uuid(Session session, string _uuid)
        {
            return session.JsonRpcClient.vusb_get_by_uuid(session.opaque_ref, _uuid);
        }

        /// <summary>
        /// Get the uuid field of the given VUSB.
        /// First published in XenServer 7.3.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vusb">The opaque_ref of the given vusb</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_uuid(Session session, string _vusb)
        {
            return session.JsonRpcClient.vusb_get_uuid(session.opaque_ref, _vusb);
        }

        /// <summary>
        /// Get the allowed_operations field of the given VUSB.
        /// First published in XenServer 7.3.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vusb">The opaque_ref of the given vusb</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static List<vusb_operations> get_allowed_operations(Session session, string _vusb)
        {
            return session.JsonRpcClient.vusb_get_allowed_operations(session.opaque_ref, _vusb);
        }

        /// <summary>
        /// Get the current_operations field of the given VUSB.
        /// First published in XenServer 7.3.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vusb">The opaque_ref of the given vusb</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static Dictionary<string, vusb_operations> get_current_operations(Session session, string _vusb)
        {
            return session.JsonRpcClient.vusb_get_current_operations(session.opaque_ref, _vusb);
        }

        /// <summary>
        /// Get the VM field of the given VUSB.
        /// First published in XenServer 7.3.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vusb">The opaque_ref of the given vusb</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static XenRef<VM> get_VM(Session session, string _vusb)
        {
            return session.JsonRpcClient.vusb_get_vm(session.opaque_ref, _vusb);
        }

        /// <summary>
        /// Get the USB_group field of the given VUSB.
        /// First published in XenServer 7.3.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vusb">The opaque_ref of the given vusb</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static XenRef<USB_group> get_USB_group(Session session, string _vusb)
        {
            return session.JsonRpcClient.vusb_get_usb_group(session.opaque_ref, _vusb);
        }

        /// <summary>
        /// Get the other_config field of the given VUSB.
        /// First published in XenServer 7.3.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vusb">The opaque_ref of the given vusb</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static Dictionary<string, string> get_other_config(Session session, string _vusb)
        {
            return session.JsonRpcClient.vusb_get_other_config(session.opaque_ref, _vusb);
        }

        /// <summary>
        /// Get the currently_attached field of the given VUSB.
        /// First published in XenServer 7.3.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vusb">The opaque_ref of the given vusb</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static bool get_currently_attached(Session session, string _vusb)
        {
            return session.JsonRpcClient.vusb_get_currently_attached(session.opaque_ref, _vusb);
        }

        /// <summary>
        /// Set the other_config field of the given VUSB.
        /// First published in XenServer 7.3.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vusb">The opaque_ref of the given vusb</param>
        /// <param name="_other_config">New value to set</param>
        /// <remarks>
        /// Minimum allowed role: pool-admin
        /// </remarks>
        public static void set_other_config(Session session, string _vusb, Dictionary<string, string> _other_config)
        {
            session.JsonRpcClient.vusb_set_other_config(session.opaque_ref, _vusb, _other_config);
        }

        /// <summary>
        /// Add the given key-value pair to the other_config field of the given VUSB.
        /// First published in XenServer 7.3.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vusb">The opaque_ref of the given vusb</param>
        /// <param name="_key">Key to add</param>
        /// <param name="_value">Value to add</param>
        /// <remarks>
        /// Minimum allowed role: pool-admin
        /// </remarks>
        public static void add_to_other_config(Session session, string _vusb, string _key, string _value)
        {
            session.JsonRpcClient.vusb_add_to_other_config(session.opaque_ref, _vusb, _key, _value);
        }

        /// <summary>
        /// Remove the given key and its corresponding value from the other_config field of the given VUSB.  If the key is not in that Map, then do nothing.
        /// First published in XenServer 7.3.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vusb">The opaque_ref of the given vusb</param>
        /// <param name="_key">Key to remove</param>
        /// <remarks>
        /// Minimum allowed role: pool-admin
        /// </remarks>
        public static void remove_from_other_config(Session session, string _vusb, string _key)
        {
            session.JsonRpcClient.vusb_remove_from_other_config(session.opaque_ref, _vusb, _key);
        }

        /// <summary>
        /// Create a new VUSB record in the database only
        /// First published in XenServer 7.3.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The VM</param>
        /// <param name="_usb_group"></param>
        /// <param name="_other_config"></param>
        /// <remarks>
        /// Minimum allowed role: pool-admin
        /// </remarks>
        public static XenRef<VUSB> create(Session session, string _vm, string _usb_group, Dictionary<string, string> _other_config)
        {
            return session.JsonRpcClient.vusb_create(session.opaque_ref, _vm, _usb_group, _other_config);
        }

        /// <summary>
        /// Create a new VUSB record in the database only
        /// First published in XenServer 7.3.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The VM</param>
        /// <param name="_usb_group"></param>
        /// <param name="_other_config"></param>
        /// <remarks>
        /// Minimum allowed role: pool-admin
        /// </remarks>
        public static XenRef<Task> async_create(Session session, string _vm, string _usb_group, Dictionary<string, string> _other_config)
        {
          return session.JsonRpcClient.async_vusb_create(session.opaque_ref, _vm, _usb_group, _other_config);
        }

        /// <summary>
        /// Unplug the vusb device from the vm.
        /// First published in XenServer 7.3.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vusb">The opaque_ref of the given vusb</param>
        /// <remarks>
        /// Minimum allowed role: pool-admin
        /// </remarks>
        public static void unplug(Session session, string _vusb)
        {
            session.JsonRpcClient.vusb_unplug(session.opaque_ref, _vusb);
        }

        /// <summary>
        /// Unplug the vusb device from the vm.
        /// First published in XenServer 7.3.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vusb">The opaque_ref of the given vusb</param>
        /// <remarks>
        /// Minimum allowed role: pool-admin
        /// </remarks>
        public static XenRef<Task> async_unplug(Session session, string _vusb)
        {
          return session.JsonRpcClient.async_vusb_unplug(session.opaque_ref, _vusb);
        }

        /// <summary>
        /// Removes a VUSB record from the database
        /// First published in XenServer 7.3.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vusb">The opaque_ref of the given vusb</param>
        /// <remarks>
        /// Minimum allowed role: pool-admin
        /// </remarks>
        public static void destroy(Session session, string _vusb)
        {
            session.JsonRpcClient.vusb_destroy(session.opaque_ref, _vusb);
        }

        /// <summary>
        /// Removes a VUSB record from the database
        /// First published in XenServer 7.3.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vusb">The opaque_ref of the given vusb</param>
        /// <remarks>
        /// Minimum allowed role: pool-admin
        /// </remarks>
        public static XenRef<Task> async_destroy(Session session, string _vusb)
        {
          return session.JsonRpcClient.async_vusb_destroy(session.opaque_ref, _vusb);
        }

        /// <summary>
        /// Return a list of all the VUSBs known to the system.
        /// First published in XenServer 7.3.
        /// </summary>
        /// <param name="session">The session</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static List<XenRef<VUSB>> get_all(Session session)
        {
            return session.JsonRpcClient.vusb_get_all(session.opaque_ref);
        }

        /// <summary>
        /// Return a map of VUSB references to VUSB records for all VUSBs known to the system.
        /// First published in XenServer 7.3.
        /// </summary>
        /// <param name="session">The session</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static Dictionary<XenRef<VUSB>, VUSB> get_all_records(Session session)
        {
            return session.JsonRpcClient.vusb_get_all_records(session.opaque_ref);
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
        /// list of the operations allowed in this state. This list is advisory only and the server state may have changed by the time this field is read by a client.
        /// </summary>
        public virtual List<vusb_operations> allowed_operations
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
        private List<vusb_operations> _allowed_operations = new List<vusb_operations>() {};

        /// <summary>
        /// links each of the running tasks using this object (by reference) to a current_operation enum which describes the nature of the task.
        /// </summary>
        public virtual Dictionary<string, vusb_operations> current_operations
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
        private Dictionary<string, vusb_operations> _current_operations = new Dictionary<string, vusb_operations>() {};

        /// <summary>
        /// VM that owns the VUSB
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
        /// USB group used by the VUSB
        /// </summary>
        [JsonConverter(typeof(XenRefConverter<USB_group>))]
        public virtual XenRef<USB_group> USB_group
        {
            get { return _USB_group; }
            set
            {
                if (!Helper.AreEqual(value, _USB_group))
                {
                    _USB_group = value;
                    NotifyPropertyChanged("USB_group");
                }
            }
        }
        private XenRef<USB_group> _USB_group = new XenRef<USB_group>(Helper.NullOpaqueRef);

        /// <summary>
        /// Additional configuration
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
        /// is the device currently attached
        /// </summary>
        public virtual bool currently_attached
        {
            get { return _currently_attached; }
            set
            {
                if (!Helper.AreEqual(value, _currently_attached))
                {
                    _currently_attached = value;
                    NotifyPropertyChanged("currently_attached");
                }
            }
        }
        private bool _currently_attached = false;
    }
}
