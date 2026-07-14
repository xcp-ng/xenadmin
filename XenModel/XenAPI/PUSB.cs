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
    /// A physical USB device
    /// First published in XenServer 7.3.
    /// </summary>
    public partial class PUSB : XenObject<PUSB>
    {
        #region Constructors

        public PUSB()
        {
        }

        public PUSB(string uuid,
            XenRef<USB_group> USB_group,
            XenRef<Host> host,
            string path,
            string vendor_id,
            string vendor_desc,
            string product_id,
            string product_desc,
            string serial,
            string version,
            string description,
            bool passthrough_enabled,
            Dictionary<string, string> other_config,
            double speed)
        {
            this.uuid = uuid;
            this.USB_group = USB_group;
            this.host = host;
            this.path = path;
            this.vendor_id = vendor_id;
            this.vendor_desc = vendor_desc;
            this.product_id = product_id;
            this.product_desc = product_desc;
            this.serial = serial;
            this.version = version;
            this.description = description;
            this.passthrough_enabled = passthrough_enabled;
            this.other_config = other_config;
            this.speed = speed;
        }

        /// <summary>
        /// Creates a new PUSB from a Hashtable.
        /// Note that the fields not contained in the Hashtable
        /// will be created with their default values.
        /// </summary>
        /// <param name="table"></param>
        public PUSB(Hashtable table)
            : this()
        {
            UpdateFrom(table);
        }

        #endregion

        /// <summary>
        /// Updates each field of this instance with the value of
        /// the corresponding field of a given PUSB.
        /// </summary>
        public override void UpdateFrom(PUSB record)
        {
            uuid = record.uuid;
            USB_group = record.USB_group;
            host = record.host;
            path = record.path;
            vendor_id = record.vendor_id;
            vendor_desc = record.vendor_desc;
            product_id = record.product_id;
            product_desc = record.product_desc;
            serial = record.serial;
            version = record.version;
            description = record.description;
            passthrough_enabled = record.passthrough_enabled;
            other_config = record.other_config;
            speed = record.speed;
        }

        /// <summary>
        /// Given a Hashtable with field-value pairs, it updates the fields of this PUSB
        /// with the values listed in the Hashtable. Note that only the fields contained
        /// in the Hashtable will be updated and the rest will remain the same.
        /// </summary>
        /// <param name="table"></param>
        public void UpdateFrom(Hashtable table)
        {
            if (table.ContainsKey("uuid"))
                uuid = Marshalling.ParseString(table, "uuid");
            if (table.ContainsKey("USB_group"))
                USB_group = Marshalling.ParseRef<USB_group>(table, "USB_group");
            if (table.ContainsKey("host"))
                host = Marshalling.ParseRef<Host>(table, "host");
            if (table.ContainsKey("path"))
                path = Marshalling.ParseString(table, "path");
            if (table.ContainsKey("vendor_id"))
                vendor_id = Marshalling.ParseString(table, "vendor_id");
            if (table.ContainsKey("vendor_desc"))
                vendor_desc = Marshalling.ParseString(table, "vendor_desc");
            if (table.ContainsKey("product_id"))
                product_id = Marshalling.ParseString(table, "product_id");
            if (table.ContainsKey("product_desc"))
                product_desc = Marshalling.ParseString(table, "product_desc");
            if (table.ContainsKey("serial"))
                serial = Marshalling.ParseString(table, "serial");
            if (table.ContainsKey("version"))
                version = Marshalling.ParseString(table, "version");
            if (table.ContainsKey("description"))
                description = Marshalling.ParseString(table, "description");
            if (table.ContainsKey("passthrough_enabled"))
                passthrough_enabled = Marshalling.ParseBool(table, "passthrough_enabled");
            if (table.ContainsKey("other_config"))
                other_config = Maps.ToDictionary_string_string(Marshalling.ParseHashTable(table, "other_config"));
            if (table.ContainsKey("speed"))
                speed = Marshalling.ParseDouble(table, "speed");
        }

        public bool DeepEquals(PUSB other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;

            return Helper.AreEqual2(_uuid, other._uuid) &&
                Helper.AreEqual2(_USB_group, other._USB_group) &&
                Helper.AreEqual2(_host, other._host) &&
                Helper.AreEqual2(_path, other._path) &&
                Helper.AreEqual2(_vendor_id, other._vendor_id) &&
                Helper.AreEqual2(_vendor_desc, other._vendor_desc) &&
                Helper.AreEqual2(_product_id, other._product_id) &&
                Helper.AreEqual2(_product_desc, other._product_desc) &&
                Helper.AreEqual2(_serial, other._serial) &&
                Helper.AreEqual2(_version, other._version) &&
                Helper.AreEqual2(_description, other._description) &&
                Helper.AreEqual2(_passthrough_enabled, other._passthrough_enabled) &&
                Helper.AreEqual2(_other_config, other._other_config) &&
                Helper.AreEqual2(_speed, other._speed);
        }


        /// <summary>
        /// Get a record containing the current state of the given PUSB.
        /// First published in XenServer 7.3.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pusb">The opaque_ref of the given pusb</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static PUSB get_record(Session session, string _pusb)
        {
            return session.JsonRpcClient.pusb_get_record(session.opaque_ref, _pusb);
        }

        /// <summary>
        /// Get a reference to the PUSB instance with the specified UUID.
        /// First published in XenServer 7.3.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_uuid">UUID of object to return</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static XenRef<PUSB> get_by_uuid(Session session, string _uuid)
        {
            return session.JsonRpcClient.pusb_get_by_uuid(session.opaque_ref, _uuid);
        }

        /// <summary>
        /// Get the uuid field of the given PUSB.
        /// First published in XenServer 7.3.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pusb">The opaque_ref of the given pusb</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_uuid(Session session, string _pusb)
        {
            return session.JsonRpcClient.pusb_get_uuid(session.opaque_ref, _pusb);
        }

        /// <summary>
        /// Get the USB_group field of the given PUSB.
        /// First published in XenServer 7.3.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pusb">The opaque_ref of the given pusb</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static XenRef<USB_group> get_USB_group(Session session, string _pusb)
        {
            return session.JsonRpcClient.pusb_get_usb_group(session.opaque_ref, _pusb);
        }

        /// <summary>
        /// Get the host field of the given PUSB.
        /// First published in XenServer 7.3.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pusb">The opaque_ref of the given pusb</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static XenRef<Host> get_host(Session session, string _pusb)
        {
            return session.JsonRpcClient.pusb_get_host(session.opaque_ref, _pusb);
        }

        /// <summary>
        /// Get the path field of the given PUSB.
        /// First published in XenServer 7.3.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pusb">The opaque_ref of the given pusb</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_path(Session session, string _pusb)
        {
            return session.JsonRpcClient.pusb_get_path(session.opaque_ref, _pusb);
        }

        /// <summary>
        /// Get the vendor_id field of the given PUSB.
        /// First published in XenServer 7.3.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pusb">The opaque_ref of the given pusb</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_vendor_id(Session session, string _pusb)
        {
            return session.JsonRpcClient.pusb_get_vendor_id(session.opaque_ref, _pusb);
        }

        /// <summary>
        /// Get the vendor_desc field of the given PUSB.
        /// First published in XenServer 7.3.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pusb">The opaque_ref of the given pusb</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_vendor_desc(Session session, string _pusb)
        {
            return session.JsonRpcClient.pusb_get_vendor_desc(session.opaque_ref, _pusb);
        }

        /// <summary>
        /// Get the product_id field of the given PUSB.
        /// First published in XenServer 7.3.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pusb">The opaque_ref of the given pusb</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_product_id(Session session, string _pusb)
        {
            return session.JsonRpcClient.pusb_get_product_id(session.opaque_ref, _pusb);
        }

        /// <summary>
        /// Get the product_desc field of the given PUSB.
        /// First published in XenServer 7.3.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pusb">The opaque_ref of the given pusb</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_product_desc(Session session, string _pusb)
        {
            return session.JsonRpcClient.pusb_get_product_desc(session.opaque_ref, _pusb);
        }

        /// <summary>
        /// Get the serial field of the given PUSB.
        /// First published in XenServer 7.3.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pusb">The opaque_ref of the given pusb</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_serial(Session session, string _pusb)
        {
            return session.JsonRpcClient.pusb_get_serial(session.opaque_ref, _pusb);
        }

        /// <summary>
        /// Get the version field of the given PUSB.
        /// First published in XenServer 7.3.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pusb">The opaque_ref of the given pusb</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_version(Session session, string _pusb)
        {
            return session.JsonRpcClient.pusb_get_version(session.opaque_ref, _pusb);
        }

        /// <summary>
        /// Get the description field of the given PUSB.
        /// First published in XenServer 7.3.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pusb">The opaque_ref of the given pusb</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_description(Session session, string _pusb)
        {
            return session.JsonRpcClient.pusb_get_description(session.opaque_ref, _pusb);
        }

        /// <summary>
        /// Get the passthrough_enabled field of the given PUSB.
        /// First published in XenServer 7.3.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pusb">The opaque_ref of the given pusb</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static bool get_passthrough_enabled(Session session, string _pusb)
        {
            return session.JsonRpcClient.pusb_get_passthrough_enabled(session.opaque_ref, _pusb);
        }

        /// <summary>
        /// Get the other_config field of the given PUSB.
        /// First published in XenServer 7.3.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pusb">The opaque_ref of the given pusb</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static Dictionary<string, string> get_other_config(Session session, string _pusb)
        {
            return session.JsonRpcClient.pusb_get_other_config(session.opaque_ref, _pusb);
        }

        /// <summary>
        /// Get the speed field of the given PUSB.
        /// First published in Citrix Hypervisor 8.2.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pusb">The opaque_ref of the given pusb</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static double get_speed(Session session, string _pusb)
        {
            return session.JsonRpcClient.pusb_get_speed(session.opaque_ref, _pusb);
        }

        /// <summary>
        /// Set the other_config field of the given PUSB.
        /// First published in XenServer 7.3.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pusb">The opaque_ref of the given pusb</param>
        /// <param name="_other_config">New value to set</param>
        /// <remarks>
        /// Minimum allowed role: pool-admin
        /// </remarks>
        public static void set_other_config(Session session, string _pusb, Dictionary<string, string> _other_config)
        {
            session.JsonRpcClient.pusb_set_other_config(session.opaque_ref, _pusb, _other_config);
        }

        /// <summary>
        /// Add the given key-value pair to the other_config field of the given PUSB.
        /// First published in XenServer 7.3.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pusb">The opaque_ref of the given pusb</param>
        /// <param name="_key">Key to add</param>
        /// <param name="_value">Value to add</param>
        /// <remarks>
        /// Minimum allowed role: pool-admin
        /// </remarks>
        public static void add_to_other_config(Session session, string _pusb, string _key, string _value)
        {
            session.JsonRpcClient.pusb_add_to_other_config(session.opaque_ref, _pusb, _key, _value);
        }

        /// <summary>
        /// Remove the given key and its corresponding value from the other_config field of the given PUSB.  If the key is not in that Map, then do nothing.
        /// First published in XenServer 7.3.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pusb">The opaque_ref of the given pusb</param>
        /// <param name="_key">Key to remove</param>
        /// <remarks>
        /// Minimum allowed role: pool-admin
        /// </remarks>
        public static void remove_from_other_config(Session session, string _pusb, string _key)
        {
            session.JsonRpcClient.pusb_remove_from_other_config(session.opaque_ref, _pusb, _key);
        }

        /// <summary>
        /// 
        /// First published in XenServer 7.3.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The host</param>
        /// <remarks>
        /// Minimum allowed role: pool-admin
        /// </remarks>
        public static void scan(Session session, string _host)
        {
            session.JsonRpcClient.pusb_scan(session.opaque_ref, _host);
        }

        /// <summary>
        /// 
        /// First published in XenServer 7.3.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The host</param>
        /// <remarks>
        /// Minimum allowed role: pool-admin
        /// </remarks>
        public static XenRef<Task> async_scan(Session session, string _host)
        {
          return session.JsonRpcClient.async_pusb_scan(session.opaque_ref, _host);
        }

        /// <summary>
        /// 
        /// First published in XenServer 7.3.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pusb">The opaque_ref of the given pusb</param>
        /// <param name="_value">passthrough is enabled when true and disabled with false</param>
        /// <remarks>
        /// Minimum allowed role: pool-admin
        /// </remarks>
        public static void set_passthrough_enabled(Session session, string _pusb, bool _value)
        {
            session.JsonRpcClient.pusb_set_passthrough_enabled(session.opaque_ref, _pusb, _value);
        }

        /// <summary>
        /// 
        /// First published in XenServer 7.3.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pusb">The opaque_ref of the given pusb</param>
        /// <param name="_value">passthrough is enabled when true and disabled with false</param>
        /// <remarks>
        /// Minimum allowed role: pool-admin
        /// </remarks>
        public static XenRef<Task> async_set_passthrough_enabled(Session session, string _pusb, bool _value)
        {
          return session.JsonRpcClient.async_pusb_set_passthrough_enabled(session.opaque_ref, _pusb, _value);
        }

        /// <summary>
        /// Return a list of all the PUSBs known to the system.
        /// First published in XenServer 7.3.
        /// </summary>
        /// <param name="session">The session</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static List<XenRef<PUSB>> get_all(Session session)
        {
            return session.JsonRpcClient.pusb_get_all(session.opaque_ref);
        }

        /// <summary>
        /// Return a map of PUSB references to PUSB records for all PUSBs known to the system.
        /// First published in XenServer 7.3.
        /// </summary>
        /// <param name="session">The session</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static Dictionary<XenRef<PUSB>, PUSB> get_all_records(Session session)
        {
            return session.JsonRpcClient.pusb_get_all_records(session.opaque_ref);
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
        /// USB group the PUSB is contained in
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
        private XenRef<USB_group> _USB_group = new XenRef<USB_group>("OpaqueRef:NULL");

        /// <summary>
        /// Physical machine that owns the USB device
        /// </summary>
        [JsonConverter(typeof(XenRefConverter<Host>))]
        public virtual XenRef<Host> host
        {
            get { return _host; }
            set
            {
                if (!Helper.AreEqual(value, _host))
                {
                    _host = value;
                    NotifyPropertyChanged("host");
                }
            }
        }
        private XenRef<Host> _host = new XenRef<Host>("OpaqueRef:NULL");

        /// <summary>
        /// port path of USB device
        /// </summary>
        public virtual string path
        {
            get { return _path; }
            set
            {
                if (!Helper.AreEqual(value, _path))
                {
                    _path = value;
                    NotifyPropertyChanged("path");
                }
            }
        }
        private string _path = "";

        /// <summary>
        /// vendor id of the USB device
        /// </summary>
        public virtual string vendor_id
        {
            get { return _vendor_id; }
            set
            {
                if (!Helper.AreEqual(value, _vendor_id))
                {
                    _vendor_id = value;
                    NotifyPropertyChanged("vendor_id");
                }
            }
        }
        private string _vendor_id = "";

        /// <summary>
        /// vendor description of the USB device
        /// </summary>
        public virtual string vendor_desc
        {
            get { return _vendor_desc; }
            set
            {
                if (!Helper.AreEqual(value, _vendor_desc))
                {
                    _vendor_desc = value;
                    NotifyPropertyChanged("vendor_desc");
                }
            }
        }
        private string _vendor_desc = "";

        /// <summary>
        /// product id of the USB device
        /// </summary>
        public virtual string product_id
        {
            get { return _product_id; }
            set
            {
                if (!Helper.AreEqual(value, _product_id))
                {
                    _product_id = value;
                    NotifyPropertyChanged("product_id");
                }
            }
        }
        private string _product_id = "";

        /// <summary>
        /// product description of the USB device
        /// </summary>
        public virtual string product_desc
        {
            get { return _product_desc; }
            set
            {
                if (!Helper.AreEqual(value, _product_desc))
                {
                    _product_desc = value;
                    NotifyPropertyChanged("product_desc");
                }
            }
        }
        private string _product_desc = "";

        /// <summary>
        /// serial of the USB device
        /// </summary>
        public virtual string serial
        {
            get { return _serial; }
            set
            {
                if (!Helper.AreEqual(value, _serial))
                {
                    _serial = value;
                    NotifyPropertyChanged("serial");
                }
            }
        }
        private string _serial = "";

        /// <summary>
        /// USB device version
        /// </summary>
        public virtual string version
        {
            get { return _version; }
            set
            {
                if (!Helper.AreEqual(value, _version))
                {
                    _version = value;
                    NotifyPropertyChanged("version");
                }
            }
        }
        private string _version = "";

        /// <summary>
        /// USB device description
        /// </summary>
        public virtual string description
        {
            get { return _description; }
            set
            {
                if (!Helper.AreEqual(value, _description))
                {
                    _description = value;
                    NotifyPropertyChanged("description");
                }
            }
        }
        private string _description = "";

        /// <summary>
        /// enabled for passthrough
        /// </summary>
        public virtual bool passthrough_enabled
        {
            get { return _passthrough_enabled; }
            set
            {
                if (!Helper.AreEqual(value, _passthrough_enabled))
                {
                    _passthrough_enabled = value;
                    NotifyPropertyChanged("passthrough_enabled");
                }
            }
        }
        private bool _passthrough_enabled = false;

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

        /// <summary>
        /// USB device speed
        /// First published in Citrix Hypervisor 8.2.
        /// </summary>
        public virtual double speed
        {
            get { return _speed; }
            set
            {
                if (!Helper.AreEqual(value, _speed))
                {
                    _speed = value;
                    NotifyPropertyChanged("speed");
                }
            }
        }
        private double _speed = -1.000;
    }
}
