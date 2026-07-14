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
    /// A group of compatible USBs across the resource pool
    /// First published in XenServer 7.3.
    /// </summary>
    public partial class USB_group : XenObject<USB_group>
    {
        #region Constructors

        public USB_group()
        {
        }

        public USB_group(string uuid,
            string name_label,
            string name_description,
            List<XenRef<PUSB>> PUSBs,
            List<XenRef<VUSB>> VUSBs,
            Dictionary<string, string> other_config)
        {
            this.uuid = uuid;
            this.name_label = name_label;
            this.name_description = name_description;
            this.PUSBs = PUSBs;
            this.VUSBs = VUSBs;
            this.other_config = other_config;
        }

        /// <summary>
        /// Creates a new USB_group from a Hashtable.
        /// Note that the fields not contained in the Hashtable
        /// will be created with their default values.
        /// </summary>
        /// <param name="table"></param>
        public USB_group(Hashtable table)
            : this()
        {
            UpdateFrom(table);
        }

        #endregion

        /// <summary>
        /// Updates each field of this instance with the value of
        /// the corresponding field of a given USB_group.
        /// </summary>
        public override void UpdateFrom(USB_group record)
        {
            uuid = record.uuid;
            name_label = record.name_label;
            name_description = record.name_description;
            PUSBs = record.PUSBs;
            VUSBs = record.VUSBs;
            other_config = record.other_config;
        }

        /// <summary>
        /// Given a Hashtable with field-value pairs, it updates the fields of this USB_group
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
            if (table.ContainsKey("PUSBs"))
                PUSBs = Marshalling.ParseSetRef<PUSB>(table, "PUSBs");
            if (table.ContainsKey("VUSBs"))
                VUSBs = Marshalling.ParseSetRef<VUSB>(table, "VUSBs");
            if (table.ContainsKey("other_config"))
                other_config = Maps.ToDictionary_string_string(Marshalling.ParseHashTable(table, "other_config"));
        }

        public bool DeepEquals(USB_group other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;

            return Helper.AreEqual2(_uuid, other._uuid) &&
                Helper.AreEqual2(_name_label, other._name_label) &&
                Helper.AreEqual2(_name_description, other._name_description) &&
                Helper.AreEqual2(_PUSBs, other._PUSBs) &&
                Helper.AreEqual2(_VUSBs, other._VUSBs) &&
                Helper.AreEqual2(_other_config, other._other_config);
        }


        /// <summary>
        /// Get a record containing the current state of the given USB_group.
        /// First published in XenServer 7.3.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_usb_group">The opaque_ref of the given usb_group</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static USB_group get_record(Session session, string _usb_group)
        {
            return session.JsonRpcClient.usb_group_get_record(session.opaque_ref, _usb_group);
        }

        /// <summary>
        /// Get a reference to the USB_group instance with the specified UUID.
        /// First published in XenServer 7.3.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_uuid">UUID of object to return</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static XenRef<USB_group> get_by_uuid(Session session, string _uuid)
        {
            return session.JsonRpcClient.usb_group_get_by_uuid(session.opaque_ref, _uuid);
        }

        /// <summary>
        /// Get all the USB_group instances with the given label.
        /// First published in XenServer 7.3.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_label">label of object to return</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static List<XenRef<USB_group>> get_by_name_label(Session session, string _label)
        {
            return session.JsonRpcClient.usb_group_get_by_name_label(session.opaque_ref, _label);
        }

        /// <summary>
        /// Get the uuid field of the given USB_group.
        /// First published in XenServer 7.3.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_usb_group">The opaque_ref of the given usb_group</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_uuid(Session session, string _usb_group)
        {
            return session.JsonRpcClient.usb_group_get_uuid(session.opaque_ref, _usb_group);
        }

        /// <summary>
        /// Get the name/label field of the given USB_group.
        /// First published in XenServer 7.3.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_usb_group">The opaque_ref of the given usb_group</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_name_label(Session session, string _usb_group)
        {
            return session.JsonRpcClient.usb_group_get_name_label(session.opaque_ref, _usb_group);
        }

        /// <summary>
        /// Get the name/description field of the given USB_group.
        /// First published in XenServer 7.3.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_usb_group">The opaque_ref of the given usb_group</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_name_description(Session session, string _usb_group)
        {
            return session.JsonRpcClient.usb_group_get_name_description(session.opaque_ref, _usb_group);
        }

        /// <summary>
        /// Get the PUSBs field of the given USB_group.
        /// First published in XenServer 7.3.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_usb_group">The opaque_ref of the given usb_group</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static List<XenRef<PUSB>> get_PUSBs(Session session, string _usb_group)
        {
            return session.JsonRpcClient.usb_group_get_pusbs(session.opaque_ref, _usb_group);
        }

        /// <summary>
        /// Get the VUSBs field of the given USB_group.
        /// First published in XenServer 7.3.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_usb_group">The opaque_ref of the given usb_group</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static List<XenRef<VUSB>> get_VUSBs(Session session, string _usb_group)
        {
            return session.JsonRpcClient.usb_group_get_vusbs(session.opaque_ref, _usb_group);
        }

        /// <summary>
        /// Get the other_config field of the given USB_group.
        /// First published in XenServer 7.3.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_usb_group">The opaque_ref of the given usb_group</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static Dictionary<string, string> get_other_config(Session session, string _usb_group)
        {
            return session.JsonRpcClient.usb_group_get_other_config(session.opaque_ref, _usb_group);
        }

        /// <summary>
        /// Set the name/label field of the given USB_group.
        /// First published in XenServer 7.3.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_usb_group">The opaque_ref of the given usb_group</param>
        /// <param name="_label">New value to set</param>
        /// <remarks>
        /// Minimum allowed role: pool-admin
        /// </remarks>
        public static void set_name_label(Session session, string _usb_group, string _label)
        {
            session.JsonRpcClient.usb_group_set_name_label(session.opaque_ref, _usb_group, _label);
        }

        /// <summary>
        /// Set the name/description field of the given USB_group.
        /// First published in XenServer 7.3.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_usb_group">The opaque_ref of the given usb_group</param>
        /// <param name="_description">New value to set</param>
        /// <remarks>
        /// Minimum allowed role: pool-admin
        /// </remarks>
        public static void set_name_description(Session session, string _usb_group, string _description)
        {
            session.JsonRpcClient.usb_group_set_name_description(session.opaque_ref, _usb_group, _description);
        }

        /// <summary>
        /// Set the other_config field of the given USB_group.
        /// First published in XenServer 7.3.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_usb_group">The opaque_ref of the given usb_group</param>
        /// <param name="_other_config">New value to set</param>
        /// <remarks>
        /// Minimum allowed role: pool-admin
        /// </remarks>
        public static void set_other_config(Session session, string _usb_group, Dictionary<string, string> _other_config)
        {
            session.JsonRpcClient.usb_group_set_other_config(session.opaque_ref, _usb_group, _other_config);
        }

        /// <summary>
        /// Add the given key-value pair to the other_config field of the given USB_group.
        /// First published in XenServer 7.3.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_usb_group">The opaque_ref of the given usb_group</param>
        /// <param name="_key">Key to add</param>
        /// <param name="_value">Value to add</param>
        /// <remarks>
        /// Minimum allowed role: pool-admin
        /// </remarks>
        public static void add_to_other_config(Session session, string _usb_group, string _key, string _value)
        {
            session.JsonRpcClient.usb_group_add_to_other_config(session.opaque_ref, _usb_group, _key, _value);
        }

        /// <summary>
        /// Remove the given key and its corresponding value from the other_config field of the given USB_group.  If the key is not in that Map, then do nothing.
        /// First published in XenServer 7.3.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_usb_group">The opaque_ref of the given usb_group</param>
        /// <param name="_key">Key to remove</param>
        /// <remarks>
        /// Minimum allowed role: pool-admin
        /// </remarks>
        public static void remove_from_other_config(Session session, string _usb_group, string _key)
        {
            session.JsonRpcClient.usb_group_remove_from_other_config(session.opaque_ref, _usb_group, _key);
        }

        /// <summary>
        /// 
        /// First published in XenServer 7.3.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_name_label"></param>
        /// <param name="_name_description"></param>
        /// <param name="_other_config"></param>
        /// <remarks>
        /// Minimum allowed role: pool-admin
        /// </remarks>
        public static XenRef<USB_group> create(Session session, string _name_label, string _name_description, Dictionary<string, string> _other_config)
        {
            return session.JsonRpcClient.usb_group_create(session.opaque_ref, _name_label, _name_description, _other_config);
        }

        /// <summary>
        /// 
        /// First published in XenServer 7.3.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_name_label"></param>
        /// <param name="_name_description"></param>
        /// <param name="_other_config"></param>
        /// <remarks>
        /// Minimum allowed role: pool-admin
        /// </remarks>
        public static XenRef<Task> async_create(Session session, string _name_label, string _name_description, Dictionary<string, string> _other_config)
        {
          return session.JsonRpcClient.async_usb_group_create(session.opaque_ref, _name_label, _name_description, _other_config);
        }

        /// <summary>
        /// 
        /// First published in XenServer 7.3.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_usb_group">The opaque_ref of the given usb_group</param>
        /// <remarks>
        /// Minimum allowed role: pool-admin
        /// </remarks>
        public static void destroy(Session session, string _usb_group)
        {
            session.JsonRpcClient.usb_group_destroy(session.opaque_ref, _usb_group);
        }

        /// <summary>
        /// 
        /// First published in XenServer 7.3.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_usb_group">The opaque_ref of the given usb_group</param>
        /// <remarks>
        /// Minimum allowed role: pool-admin
        /// </remarks>
        public static XenRef<Task> async_destroy(Session session, string _usb_group)
        {
          return session.JsonRpcClient.async_usb_group_destroy(session.opaque_ref, _usb_group);
        }

        /// <summary>
        /// Return a list of all the USB_groups known to the system.
        /// First published in XenServer 7.3.
        /// </summary>
        /// <param name="session">The session</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static List<XenRef<USB_group>> get_all(Session session)
        {
            return session.JsonRpcClient.usb_group_get_all(session.opaque_ref);
        }

        /// <summary>
        /// Return a map of USB_group references to USB_group records for all USB_groups known to the system.
        /// First published in XenServer 7.3.
        /// </summary>
        /// <param name="session">The session</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static Dictionary<XenRef<USB_group>, USB_group> get_all_records(Session session)
        {
            return session.JsonRpcClient.usb_group_get_all_records(session.opaque_ref);
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
        /// List of PUSBs in the group
        /// </summary>
        [JsonConverter(typeof(XenRefListConverter<PUSB>))]
        public virtual List<XenRef<PUSB>> PUSBs
        {
            get { return _PUSBs; }
            set
            {
                if (!Helper.AreEqual(value, _PUSBs))
                {
                    _PUSBs = value;
                    NotifyPropertyChanged("PUSBs");
                }
            }
        }
        private List<XenRef<PUSB>> _PUSBs = new List<XenRef<PUSB>>() {};

        /// <summary>
        /// List of VUSBs using the group
        /// </summary>
        [JsonConverter(typeof(XenRefListConverter<VUSB>))]
        public virtual List<XenRef<VUSB>> VUSBs
        {
            get { return _VUSBs; }
            set
            {
                if (!Helper.AreEqual(value, _VUSBs))
                {
                    _VUSBs = value;
                    NotifyPropertyChanged("VUSBs");
                }
            }
        }
        private List<XenRef<VUSB>> _VUSBs = new List<XenRef<VUSB>>() {};

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
    }
}
