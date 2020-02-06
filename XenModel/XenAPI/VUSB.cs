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

        /// <summary>
        /// Creates a new VUSB from a Proxy_VUSB.
        /// </summary>
        /// <param name="proxy"></param>
        public VUSB(Proxy_VUSB proxy)
        {
            UpdateFrom(proxy);
        }

        #endregion

        /// <summary>
        /// Updates each field of this instance with the value of
        /// the corresponding field of a given VUSB.
        /// </summary>
        public override void UpdateFrom(VUSB update)
        {
            uuid = update.uuid;
            allowed_operations = update.allowed_operations;
            current_operations = update.current_operations;
            VM = update.VM;
            USB_group = update.USB_group;
            other_config = update.other_config;
            currently_attached = update.currently_attached;
        }

        internal void UpdateFrom(Proxy_VUSB proxy)
        {
            uuid = proxy.uuid == null ? null : proxy.uuid;
            allowed_operations = proxy.allowed_operations == null ? null : Helper.StringArrayToEnumList<vusb_operations>(proxy.allowed_operations);
            current_operations = proxy.current_operations == null ? null : Maps.convert_from_proxy_string_vusb_operations(proxy.current_operations);
            VM = proxy.VM == null ? null : XenRef<VM>.Create(proxy.VM);
            USB_group = proxy.USB_group == null ? null : XenRef<USB_group>.Create(proxy.USB_group);
            other_config = proxy.other_config == null ? null : Maps.convert_from_proxy_string_string(proxy.other_config);
            currently_attached = (bool)proxy.currently_attached;
        }

        public Proxy_VUSB ToProxy()
        {
            Proxy_VUSB result_ = new Proxy_VUSB();
            result_.uuid = uuid ?? "";
            result_.allowed_operations = allowed_operations == null ? new string[] {} : Helper.ObjectListToStringArray(allowed_operations);
            result_.current_operations = Maps.convert_to_proxy_string_vusb_operations(current_operations);
            result_.VM = VM ?? "";
            result_.USB_group = USB_group ?? "";
            result_.other_config = Maps.convert_to_proxy_string_string(other_config);
            result_.currently_attached = currently_attached;
            return result_;
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
                current_operations = Maps.convert_from_proxy_string_vusb_operations(Marshalling.ParseHashTable(table, "current_operations"));
            if (table.ContainsKey("VM"))
                VM = Marshalling.ParseRef<VM>(table, "VM");
            if (table.ContainsKey("USB_group"))
                USB_group = Marshalling.ParseRef<USB_group>(table, "USB_group");
            if (table.ContainsKey("other_config"))
                other_config = Maps.convert_from_proxy_string_string(Marshalling.ParseHashTable(table, "other_config"));
            if (table.ContainsKey("currently_attached"))
                currently_attached = Marshalling.ParseBool(table, "currently_attached");
        }

        public bool DeepEquals(VUSB other, bool ignoreCurrentOperations)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;

            if (!ignoreCurrentOperations && !Helper.AreEqual2(this.current_operations, other.current_operations))
                return false;

            return Helper.AreEqual2(this._uuid, other._uuid) &&
                Helper.AreEqual2(this._allowed_operations, other._allowed_operations) &&
                Helper.AreEqual2(this._VM, other._VM) &&
                Helper.AreEqual2(this._USB_group, other._USB_group) &&
                Helper.AreEqual2(this._other_config, other._other_config) &&
                Helper.AreEqual2(this._currently_attached, other._currently_attached);
        }

        internal static List<VUSB> ProxyArrayToObjectList(Proxy_VUSB[] input)
        {
            var result = new List<VUSB>();
            foreach (var item in input)
                result.Add(new VUSB(item));

            return result;
        }

        public override string SaveChanges(Session session, string opaqueRef, VUSB server)
        {
            if (opaqueRef == null)
            {
                System.Diagnostics.Debug.Assert(false, "Cannot create instances of this type on the server");
                return "";
            }
            else
            {
                if (!Helper.AreEqual2(_other_config, server._other_config))
                {
                    VUSB.set_other_config(session, opaqueRef, _other_config);
                }

                return null;
            }
        }
        /// <summary>
        /// Get a record containing the current state of the given VUSB.
        /// First published in XenServer 7.3.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vusb">The opaque_ref of the given vusb</param>
        public static VUSB get_record(Session session, string _vusb)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.vusb_get_record(session.opaque_ref, _vusb);
            else
                return new VUSB(session.proxy.vusb_get_record(session.opaque_ref, _vusb ?? "").parse());
        }

        /// <summary>
        /// Get a reference to the VUSB instance with the specified UUID.
        /// First published in XenServer 7.3.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_uuid">UUID of object to return</param>
        public static XenRef<VUSB> get_by_uuid(Session session, string _uuid)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.vusb_get_by_uuid(session.opaque_ref, _uuid);
            else
                return XenRef<VUSB>.Create(session.proxy.vusb_get_by_uuid(session.opaque_ref, _uuid ?? "").parse());
        }

        /// <summary>
        /// Get the uuid field of the given VUSB.
        /// First published in XenServer 7.3.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vusb">The opaque_ref of the given vusb</param>
        public static string get_uuid(Session session, string _vusb)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.vusb_get_uuid(session.opaque_ref, _vusb);
            else
                return session.proxy.vusb_get_uuid(session.opaque_ref, _vusb ?? "").parse();
        }

        /// <summary>
        /// Get the allowed_operations field of the given VUSB.
        /// First published in XenServer 7.3.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vusb">The opaque_ref of the given vusb</param>
        public static List<vusb_operations> get_allowed_operations(Session session, string _vusb)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.vusb_get_allowed_operations(session.opaque_ref, _vusb);
            else
                return Helper.StringArrayToEnumList<vusb_operations>(session.proxy.vusb_get_allowed_operations(session.opaque_ref, _vusb ?? "").parse());
        }

        /// <summary>
        /// Get the current_operations field of the given VUSB.
        /// First published in XenServer 7.3.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vusb">The opaque_ref of the given vusb</param>
        public static Dictionary<string, vusb_operations> get_current_operations(Session session, string _vusb)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.vusb_get_current_operations(session.opaque_ref, _vusb);
            else
                return Maps.convert_from_proxy_string_vusb_operations(session.proxy.vusb_get_current_operations(session.opaque_ref, _vusb ?? "").parse());
        }

        /// <summary>
        /// Get the VM field of the given VUSB.
        /// First published in XenServer 7.3.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vusb">The opaque_ref of the given vusb</param>
        public static XenRef<VM> get_VM(Session session, string _vusb)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.vusb_get_vm(session.opaque_ref, _vusb);
            else
                return XenRef<VM>.Create(session.proxy.vusb_get_vm(session.opaque_ref, _vusb ?? "").parse());
        }

        /// <summary>
        /// Get the USB_group field of the given VUSB.
        /// First published in XenServer 7.3.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vusb">The opaque_ref of the given vusb</param>
        public static XenRef<USB_group> get_USB_group(Session session, string _vusb)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.vusb_get_usb_group(session.opaque_ref, _vusb);
            else
                return XenRef<USB_group>.Create(session.proxy.vusb_get_usb_group(session.opaque_ref, _vusb ?? "").parse());
        }

        /// <summary>
        /// Get the other_config field of the given VUSB.
        /// First published in XenServer 7.3.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vusb">The opaque_ref of the given vusb</param>
        public static Dictionary<string, string> get_other_config(Session session, string _vusb)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.vusb_get_other_config(session.opaque_ref, _vusb);
            else
                return Maps.convert_from_proxy_string_string(session.proxy.vusb_get_other_config(session.opaque_ref, _vusb ?? "").parse());
        }

        /// <summary>
        /// Get the currently_attached field of the given VUSB.
        /// First published in XenServer 7.3.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vusb">The opaque_ref of the given vusb</param>
        public static bool get_currently_attached(Session session, string _vusb)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.vusb_get_currently_attached(session.opaque_ref, _vusb);
            else
                return (bool)session.proxy.vusb_get_currently_attached(session.opaque_ref, _vusb ?? "").parse();
        }

        /// <summary>
        /// Set the other_config field of the given VUSB.
        /// First published in XenServer 7.3.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vusb">The opaque_ref of the given vusb</param>
        /// <param name="_other_config">New value to set</param>
        public static void set_other_config(Session session, string _vusb, Dictionary<string, string> _other_config)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.vusb_set_other_config(session.opaque_ref, _vusb, _other_config);
            else
                session.proxy.vusb_set_other_config(session.opaque_ref, _vusb ?? "", Maps.convert_to_proxy_string_string(_other_config)).parse();
        }

        /// <summary>
        /// Add the given key-value pair to the other_config field of the given VUSB.
        /// First published in XenServer 7.3.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vusb">The opaque_ref of the given vusb</param>
        /// <param name="_key">Key to add</param>
        /// <param name="_value">Value to add</param>
        public static void add_to_other_config(Session session, string _vusb, string _key, string _value)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.vusb_add_to_other_config(session.opaque_ref, _vusb, _key, _value);
            else
                session.proxy.vusb_add_to_other_config(session.opaque_ref, _vusb ?? "", _key ?? "", _value ?? "").parse();
        }

        /// <summary>
        /// Remove the given key and its corresponding value from the other_config field of the given VUSB.  If the key is not in that Map, then do nothing.
        /// First published in XenServer 7.3.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vusb">The opaque_ref of the given vusb</param>
        /// <param name="_key">Key to remove</param>
        public static void remove_from_other_config(Session session, string _vusb, string _key)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.vusb_remove_from_other_config(session.opaque_ref, _vusb, _key);
            else
                session.proxy.vusb_remove_from_other_config(session.opaque_ref, _vusb ?? "", _key ?? "").parse();
        }

        /// <summary>
        /// Create a new VUSB record in the database only
        /// First published in XenServer 7.3.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The VM</param>
        /// <param name="_usb_group"></param>
        /// <param name="_other_config"></param>
        public static XenRef<VUSB> create(Session session, string _vm, string _usb_group, Dictionary<string, string> _other_config)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.vusb_create(session.opaque_ref, _vm, _usb_group, _other_config);
            else
                return XenRef<VUSB>.Create(session.proxy.vusb_create(session.opaque_ref, _vm ?? "", _usb_group ?? "", Maps.convert_to_proxy_string_string(_other_config)).parse());
        }

        /// <summary>
        /// Create a new VUSB record in the database only
        /// First published in XenServer 7.3.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm">The VM</param>
        /// <param name="_usb_group"></param>
        /// <param name="_other_config"></param>
        public static XenRef<Task> async_create(Session session, string _vm, string _usb_group, Dictionary<string, string> _other_config)
        {
          if (session.JsonRpcClient != null)
              return session.JsonRpcClient.async_vusb_create(session.opaque_ref, _vm, _usb_group, _other_config);
          else
              return XenRef<Task>.Create(session.proxy.async_vusb_create(session.opaque_ref, _vm ?? "", _usb_group ?? "", Maps.convert_to_proxy_string_string(_other_config)).parse());
        }

        /// <summary>
        /// Unplug the vusb device from the vm.
        /// First published in XenServer 7.3.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vusb">The opaque_ref of the given vusb</param>
        public static void unplug(Session session, string _vusb)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.vusb_unplug(session.opaque_ref, _vusb);
            else
                session.proxy.vusb_unplug(session.opaque_ref, _vusb ?? "").parse();
        }

        /// <summary>
        /// Unplug the vusb device from the vm.
        /// First published in XenServer 7.3.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vusb">The opaque_ref of the given vusb</param>
        public static XenRef<Task> async_unplug(Session session, string _vusb)
        {
          if (session.JsonRpcClient != null)
              return session.JsonRpcClient.async_vusb_unplug(session.opaque_ref, _vusb);
          else
              return XenRef<Task>.Create(session.proxy.async_vusb_unplug(session.opaque_ref, _vusb ?? "").parse());
        }

        /// <summary>
        /// Removes a VUSB record from the database
        /// First published in XenServer 7.3.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vusb">The opaque_ref of the given vusb</param>
        public static void destroy(Session session, string _vusb)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.vusb_destroy(session.opaque_ref, _vusb);
            else
                session.proxy.vusb_destroy(session.opaque_ref, _vusb ?? "").parse();
        }

        /// <summary>
        /// Removes a VUSB record from the database
        /// First published in XenServer 7.3.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vusb">The opaque_ref of the given vusb</param>
        public static XenRef<Task> async_destroy(Session session, string _vusb)
        {
          if (session.JsonRpcClient != null)
              return session.JsonRpcClient.async_vusb_destroy(session.opaque_ref, _vusb);
          else
              return XenRef<Task>.Create(session.proxy.async_vusb_destroy(session.opaque_ref, _vusb ?? "").parse());
        }

        /// <summary>
        /// Return a list of all the VUSBs known to the system.
        /// First published in XenServer 7.3.
        /// </summary>
        /// <param name="session">The session</param>
        public static List<XenRef<VUSB>> get_all(Session session)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.vusb_get_all(session.opaque_ref);
            else
                return XenRef<VUSB>.Create(session.proxy.vusb_get_all(session.opaque_ref).parse());
        }

        /// <summary>
        /// Get all the VUSB Records at once, in a single XML RPC call
        /// First published in XenServer 7.3.
        /// </summary>
        /// <param name="session">The session</param>
        public static Dictionary<XenRef<VUSB>, VUSB> get_all_records(Session session)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.vusb_get_all_records(session.opaque_ref);
            else
                return XenRef<VUSB>.Create<Proxy_VUSB>(session.proxy.vusb_get_all_records(session.opaque_ref).parse());
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
                    Changed = true;
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
                    Changed = true;
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
                    Changed = true;
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
                    Changed = true;
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
                    Changed = true;
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
                    Changed = true;
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
                    Changed = true;
                    NotifyPropertyChanged("currently_attached");
                }
            }
        }
        private bool _currently_attached = false;
    }
}
