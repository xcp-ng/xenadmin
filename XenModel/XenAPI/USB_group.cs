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

        /// <summary>
        /// Creates a new USB_group from a Proxy_USB_group.
        /// </summary>
        /// <param name="proxy"></param>
        public USB_group(Proxy_USB_group proxy)
        {
            UpdateFrom(proxy);
        }

        #endregion

        /// <summary>
        /// Updates each field of this instance with the value of
        /// the corresponding field of a given USB_group.
        /// </summary>
        public override void UpdateFrom(USB_group update)
        {
            uuid = update.uuid;
            name_label = update.name_label;
            name_description = update.name_description;
            PUSBs = update.PUSBs;
            VUSBs = update.VUSBs;
            other_config = update.other_config;
        }

        internal void UpdateFrom(Proxy_USB_group proxy)
        {
            uuid = proxy.uuid == null ? null : proxy.uuid;
            name_label = proxy.name_label == null ? null : proxy.name_label;
            name_description = proxy.name_description == null ? null : proxy.name_description;
            PUSBs = proxy.PUSBs == null ? null : XenRef<PUSB>.Create(proxy.PUSBs);
            VUSBs = proxy.VUSBs == null ? null : XenRef<VUSB>.Create(proxy.VUSBs);
            other_config = proxy.other_config == null ? null : Maps.convert_from_proxy_string_string(proxy.other_config);
        }

        public Proxy_USB_group ToProxy()
        {
            Proxy_USB_group result_ = new Proxy_USB_group();
            result_.uuid = uuid ?? "";
            result_.name_label = name_label ?? "";
            result_.name_description = name_description ?? "";
            result_.PUSBs = PUSBs == null ? new string[] {} : Helper.RefListToStringArray(PUSBs);
            result_.VUSBs = VUSBs == null ? new string[] {} : Helper.RefListToStringArray(VUSBs);
            result_.other_config = Maps.convert_to_proxy_string_string(other_config);
            return result_;
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
                other_config = Maps.convert_from_proxy_string_string(Marshalling.ParseHashTable(table, "other_config"));
        }

        public bool DeepEquals(USB_group other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;

            return Helper.AreEqual2(this._uuid, other._uuid) &&
                Helper.AreEqual2(this._name_label, other._name_label) &&
                Helper.AreEqual2(this._name_description, other._name_description) &&
                Helper.AreEqual2(this._PUSBs, other._PUSBs) &&
                Helper.AreEqual2(this._VUSBs, other._VUSBs) &&
                Helper.AreEqual2(this._other_config, other._other_config);
        }

        internal static List<USB_group> ProxyArrayToObjectList(Proxy_USB_group[] input)
        {
            var result = new List<USB_group>();
            foreach (var item in input)
                result.Add(new USB_group(item));

            return result;
        }

        public override string SaveChanges(Session session, string opaqueRef, USB_group server)
        {
            if (opaqueRef == null)
            {
                System.Diagnostics.Debug.Assert(false, "Cannot create instances of this type on the server");
                return "";
            }
            else
            {
                if (!Helper.AreEqual2(_name_label, server._name_label))
                {
                    USB_group.set_name_label(session, opaqueRef, _name_label);
                }
                if (!Helper.AreEqual2(_name_description, server._name_description))
                {
                    USB_group.set_name_description(session, opaqueRef, _name_description);
                }
                if (!Helper.AreEqual2(_other_config, server._other_config))
                {
                    USB_group.set_other_config(session, opaqueRef, _other_config);
                }

                return null;
            }
        }
        /// <summary>
        /// Get a record containing the current state of the given USB_group.
        /// First published in XenServer 7.3.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_usb_group">The opaque_ref of the given usb_group</param>
        public static USB_group get_record(Session session, string _usb_group)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.usb_group_get_record(session.opaque_ref, _usb_group);
            else
                return new USB_group(session.XmlRpcProxy.usb_group_get_record(session.opaque_ref, _usb_group ?? "").parse());
        }

        /// <summary>
        /// Get a reference to the USB_group instance with the specified UUID.
        /// First published in XenServer 7.3.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_uuid">UUID of object to return</param>
        public static XenRef<USB_group> get_by_uuid(Session session, string _uuid)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.usb_group_get_by_uuid(session.opaque_ref, _uuid);
            else
                return XenRef<USB_group>.Create(session.XmlRpcProxy.usb_group_get_by_uuid(session.opaque_ref, _uuid ?? "").parse());
        }

        /// <summary>
        /// Get all the USB_group instances with the given label.
        /// First published in XenServer 7.3.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_label">label of object to return</param>
        public static List<XenRef<USB_group>> get_by_name_label(Session session, string _label)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.usb_group_get_by_name_label(session.opaque_ref, _label);
            else
                return XenRef<USB_group>.Create(session.XmlRpcProxy.usb_group_get_by_name_label(session.opaque_ref, _label ?? "").parse());
        }

        /// <summary>
        /// Get the uuid field of the given USB_group.
        /// First published in XenServer 7.3.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_usb_group">The opaque_ref of the given usb_group</param>
        public static string get_uuid(Session session, string _usb_group)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.usb_group_get_uuid(session.opaque_ref, _usb_group);
            else
                return session.XmlRpcProxy.usb_group_get_uuid(session.opaque_ref, _usb_group ?? "").parse();
        }

        /// <summary>
        /// Get the name/label field of the given USB_group.
        /// First published in XenServer 7.3.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_usb_group">The opaque_ref of the given usb_group</param>
        public static string get_name_label(Session session, string _usb_group)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.usb_group_get_name_label(session.opaque_ref, _usb_group);
            else
                return session.XmlRpcProxy.usb_group_get_name_label(session.opaque_ref, _usb_group ?? "").parse();
        }

        /// <summary>
        /// Get the name/description field of the given USB_group.
        /// First published in XenServer 7.3.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_usb_group">The opaque_ref of the given usb_group</param>
        public static string get_name_description(Session session, string _usb_group)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.usb_group_get_name_description(session.opaque_ref, _usb_group);
            else
                return session.XmlRpcProxy.usb_group_get_name_description(session.opaque_ref, _usb_group ?? "").parse();
        }

        /// <summary>
        /// Get the PUSBs field of the given USB_group.
        /// First published in XenServer 7.3.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_usb_group">The opaque_ref of the given usb_group</param>
        public static List<XenRef<PUSB>> get_PUSBs(Session session, string _usb_group)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.usb_group_get_pusbs(session.opaque_ref, _usb_group);
            else
                return XenRef<PUSB>.Create(session.XmlRpcProxy.usb_group_get_pusbs(session.opaque_ref, _usb_group ?? "").parse());
        }

        /// <summary>
        /// Get the VUSBs field of the given USB_group.
        /// First published in XenServer 7.3.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_usb_group">The opaque_ref of the given usb_group</param>
        public static List<XenRef<VUSB>> get_VUSBs(Session session, string _usb_group)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.usb_group_get_vusbs(session.opaque_ref, _usb_group);
            else
                return XenRef<VUSB>.Create(session.XmlRpcProxy.usb_group_get_vusbs(session.opaque_ref, _usb_group ?? "").parse());
        }

        /// <summary>
        /// Get the other_config field of the given USB_group.
        /// First published in XenServer 7.3.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_usb_group">The opaque_ref of the given usb_group</param>
        public static Dictionary<string, string> get_other_config(Session session, string _usb_group)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.usb_group_get_other_config(session.opaque_ref, _usb_group);
            else
                return Maps.convert_from_proxy_string_string(session.XmlRpcProxy.usb_group_get_other_config(session.opaque_ref, _usb_group ?? "").parse());
        }

        /// <summary>
        /// Set the name/label field of the given USB_group.
        /// First published in XenServer 7.3.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_usb_group">The opaque_ref of the given usb_group</param>
        /// <param name="_label">New value to set</param>
        public static void set_name_label(Session session, string _usb_group, string _label)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.usb_group_set_name_label(session.opaque_ref, _usb_group, _label);
            else
                session.XmlRpcProxy.usb_group_set_name_label(session.opaque_ref, _usb_group ?? "", _label ?? "").parse();
        }

        /// <summary>
        /// Set the name/description field of the given USB_group.
        /// First published in XenServer 7.3.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_usb_group">The opaque_ref of the given usb_group</param>
        /// <param name="_description">New value to set</param>
        public static void set_name_description(Session session, string _usb_group, string _description)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.usb_group_set_name_description(session.opaque_ref, _usb_group, _description);
            else
                session.XmlRpcProxy.usb_group_set_name_description(session.opaque_ref, _usb_group ?? "", _description ?? "").parse();
        }

        /// <summary>
        /// Set the other_config field of the given USB_group.
        /// First published in XenServer 7.3.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_usb_group">The opaque_ref of the given usb_group</param>
        /// <param name="_other_config">New value to set</param>
        public static void set_other_config(Session session, string _usb_group, Dictionary<string, string> _other_config)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.usb_group_set_other_config(session.opaque_ref, _usb_group, _other_config);
            else
                session.XmlRpcProxy.usb_group_set_other_config(session.opaque_ref, _usb_group ?? "", Maps.convert_to_proxy_string_string(_other_config)).parse();
        }

        /// <summary>
        /// Add the given key-value pair to the other_config field of the given USB_group.
        /// First published in XenServer 7.3.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_usb_group">The opaque_ref of the given usb_group</param>
        /// <param name="_key">Key to add</param>
        /// <param name="_value">Value to add</param>
        public static void add_to_other_config(Session session, string _usb_group, string _key, string _value)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.usb_group_add_to_other_config(session.opaque_ref, _usb_group, _key, _value);
            else
                session.XmlRpcProxy.usb_group_add_to_other_config(session.opaque_ref, _usb_group ?? "", _key ?? "", _value ?? "").parse();
        }

        /// <summary>
        /// Remove the given key and its corresponding value from the other_config field of the given USB_group.  If the key is not in that Map, then do nothing.
        /// First published in XenServer 7.3.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_usb_group">The opaque_ref of the given usb_group</param>
        /// <param name="_key">Key to remove</param>
        public static void remove_from_other_config(Session session, string _usb_group, string _key)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.usb_group_remove_from_other_config(session.opaque_ref, _usb_group, _key);
            else
                session.XmlRpcProxy.usb_group_remove_from_other_config(session.opaque_ref, _usb_group ?? "", _key ?? "").parse();
        }

        /// <summary>
        /// 
        /// First published in XenServer 7.3.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_name_label"></param>
        /// <param name="_name_description"></param>
        /// <param name="_other_config"></param>
        public static XenRef<USB_group> create(Session session, string _name_label, string _name_description, Dictionary<string, string> _other_config)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.usb_group_create(session.opaque_ref, _name_label, _name_description, _other_config);
            else
                return XenRef<USB_group>.Create(session.XmlRpcProxy.usb_group_create(session.opaque_ref, _name_label ?? "", _name_description ?? "", Maps.convert_to_proxy_string_string(_other_config)).parse());
        }

        /// <summary>
        /// 
        /// First published in XenServer 7.3.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_name_label"></param>
        /// <param name="_name_description"></param>
        /// <param name="_other_config"></param>
        public static XenRef<Task> async_create(Session session, string _name_label, string _name_description, Dictionary<string, string> _other_config)
        {
          if (session.JsonRpcClient != null)
              return session.JsonRpcClient.async_usb_group_create(session.opaque_ref, _name_label, _name_description, _other_config);
          else
              return XenRef<Task>.Create(session.XmlRpcProxy.async_usb_group_create(session.opaque_ref, _name_label ?? "", _name_description ?? "", Maps.convert_to_proxy_string_string(_other_config)).parse());
        }

        /// <summary>
        /// 
        /// First published in XenServer 7.3.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_usb_group">The opaque_ref of the given usb_group</param>
        public static void destroy(Session session, string _usb_group)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.usb_group_destroy(session.opaque_ref, _usb_group);
            else
                session.XmlRpcProxy.usb_group_destroy(session.opaque_ref, _usb_group ?? "").parse();
        }

        /// <summary>
        /// 
        /// First published in XenServer 7.3.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_usb_group">The opaque_ref of the given usb_group</param>
        public static XenRef<Task> async_destroy(Session session, string _usb_group)
        {
          if (session.JsonRpcClient != null)
              return session.JsonRpcClient.async_usb_group_destroy(session.opaque_ref, _usb_group);
          else
              return XenRef<Task>.Create(session.XmlRpcProxy.async_usb_group_destroy(session.opaque_ref, _usb_group ?? "").parse());
        }

        /// <summary>
        /// Return a list of all the USB_groups known to the system.
        /// First published in XenServer 7.3.
        /// </summary>
        /// <param name="session">The session</param>
        public static List<XenRef<USB_group>> get_all(Session session)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.usb_group_get_all(session.opaque_ref);
            else
                return XenRef<USB_group>.Create(session.XmlRpcProxy.usb_group_get_all(session.opaque_ref).parse());
        }

        /// <summary>
        /// Get all the USB_group Records at once, in a single XML RPC call
        /// First published in XenServer 7.3.
        /// </summary>
        /// <param name="session">The session</param>
        public static Dictionary<XenRef<USB_group>, USB_group> get_all_records(Session session)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.usb_group_get_all_records(session.opaque_ref);
            else
                return XenRef<USB_group>.Create<Proxy_USB_group>(session.XmlRpcProxy.usb_group_get_all_records(session.opaque_ref).parse());
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
