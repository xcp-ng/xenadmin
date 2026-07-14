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
    /// A PCI device
    /// First published in XenServer 6.0.
    /// </summary>
    public partial class PCI : XenObject<PCI>
    {
        #region Constructors

        public PCI()
        {
        }

        public PCI(string uuid,
            string class_id,
            string class_name,
            string vendor_id,
            string vendor_name,
            string device_id,
            string device_name,
            XenRef<Host> host,
            string pci_id,
            List<XenRef<PCI>> dependencies,
            Dictionary<string, string> other_config,
            string subsystem_vendor_id,
            string subsystem_vendor_name,
            string subsystem_device_id,
            string subsystem_device_name,
            string driver_name)
        {
            this.uuid = uuid;
            this.class_id = class_id;
            this.class_name = class_name;
            this.vendor_id = vendor_id;
            this.vendor_name = vendor_name;
            this.device_id = device_id;
            this.device_name = device_name;
            this.host = host;
            this.pci_id = pci_id;
            this.dependencies = dependencies;
            this.other_config = other_config;
            this.subsystem_vendor_id = subsystem_vendor_id;
            this.subsystem_vendor_name = subsystem_vendor_name;
            this.subsystem_device_id = subsystem_device_id;
            this.subsystem_device_name = subsystem_device_name;
            this.driver_name = driver_name;
        }

        /// <summary>
        /// Creates a new PCI from a Hashtable.
        /// Note that the fields not contained in the Hashtable
        /// will be created with their default values.
        /// </summary>
        /// <param name="table"></param>
        public PCI(Hashtable table)
            : this()
        {
            UpdateFrom(table);
        }

        #endregion

        /// <summary>
        /// Updates each field of this instance with the value of
        /// the corresponding field of a given PCI.
        /// </summary>
        public override void UpdateFrom(PCI record)
        {
            uuid = record.uuid;
            class_id = record.class_id;
            class_name = record.class_name;
            vendor_id = record.vendor_id;
            vendor_name = record.vendor_name;
            device_id = record.device_id;
            device_name = record.device_name;
            host = record.host;
            pci_id = record.pci_id;
            dependencies = record.dependencies;
            other_config = record.other_config;
            subsystem_vendor_id = record.subsystem_vendor_id;
            subsystem_vendor_name = record.subsystem_vendor_name;
            subsystem_device_id = record.subsystem_device_id;
            subsystem_device_name = record.subsystem_device_name;
            driver_name = record.driver_name;
        }

        /// <summary>
        /// Given a Hashtable with field-value pairs, it updates the fields of this PCI
        /// with the values listed in the Hashtable. Note that only the fields contained
        /// in the Hashtable will be updated and the rest will remain the same.
        /// </summary>
        /// <param name="table"></param>
        public void UpdateFrom(Hashtable table)
        {
            if (table.ContainsKey("uuid"))
                uuid = Marshalling.ParseString(table, "uuid");
            if (table.ContainsKey("class_id"))
                class_id = Marshalling.ParseString(table, "class_id");
            if (table.ContainsKey("class_name"))
                class_name = Marshalling.ParseString(table, "class_name");
            if (table.ContainsKey("vendor_id"))
                vendor_id = Marshalling.ParseString(table, "vendor_id");
            if (table.ContainsKey("vendor_name"))
                vendor_name = Marshalling.ParseString(table, "vendor_name");
            if (table.ContainsKey("device_id"))
                device_id = Marshalling.ParseString(table, "device_id");
            if (table.ContainsKey("device_name"))
                device_name = Marshalling.ParseString(table, "device_name");
            if (table.ContainsKey("host"))
                host = Marshalling.ParseRef<Host>(table, "host");
            if (table.ContainsKey("pci_id"))
                pci_id = Marshalling.ParseString(table, "pci_id");
            if (table.ContainsKey("dependencies"))
                dependencies = Marshalling.ParseSetRef<PCI>(table, "dependencies");
            if (table.ContainsKey("other_config"))
                other_config = Maps.ToDictionary_string_string(Marshalling.ParseHashTable(table, "other_config"));
            if (table.ContainsKey("subsystem_vendor_id"))
                subsystem_vendor_id = Marshalling.ParseString(table, "subsystem_vendor_id");
            if (table.ContainsKey("subsystem_vendor_name"))
                subsystem_vendor_name = Marshalling.ParseString(table, "subsystem_vendor_name");
            if (table.ContainsKey("subsystem_device_id"))
                subsystem_device_id = Marshalling.ParseString(table, "subsystem_device_id");
            if (table.ContainsKey("subsystem_device_name"))
                subsystem_device_name = Marshalling.ParseString(table, "subsystem_device_name");
            if (table.ContainsKey("driver_name"))
                driver_name = Marshalling.ParseString(table, "driver_name");
        }

        public bool DeepEquals(PCI other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;

            return Helper.AreEqual2(_uuid, other._uuid) &&
                Helper.AreEqual2(_class_id, other._class_id) &&
                Helper.AreEqual2(_class_name, other._class_name) &&
                Helper.AreEqual2(_vendor_id, other._vendor_id) &&
                Helper.AreEqual2(_vendor_name, other._vendor_name) &&
                Helper.AreEqual2(_device_id, other._device_id) &&
                Helper.AreEqual2(_device_name, other._device_name) &&
                Helper.AreEqual2(_host, other._host) &&
                Helper.AreEqual2(_pci_id, other._pci_id) &&
                Helper.AreEqual2(_dependencies, other._dependencies) &&
                Helper.AreEqual2(_other_config, other._other_config) &&
                Helper.AreEqual2(_subsystem_vendor_id, other._subsystem_vendor_id) &&
                Helper.AreEqual2(_subsystem_vendor_name, other._subsystem_vendor_name) &&
                Helper.AreEqual2(_subsystem_device_id, other._subsystem_device_id) &&
                Helper.AreEqual2(_subsystem_device_name, other._subsystem_device_name) &&
                Helper.AreEqual2(_driver_name, other._driver_name);
        }


        /// <summary>
        /// Get a record containing the current state of the given PCI.
        /// First published in XenServer 6.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pci">The opaque_ref of the given pci</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static PCI get_record(Session session, string _pci)
        {
            return session.JsonRpcClient.pci_get_record(session.opaque_ref, _pci);
        }

        /// <summary>
        /// Get a reference to the PCI instance with the specified UUID.
        /// First published in XenServer 6.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_uuid">UUID of object to return</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static XenRef<PCI> get_by_uuid(Session session, string _uuid)
        {
            return session.JsonRpcClient.pci_get_by_uuid(session.opaque_ref, _uuid);
        }

        /// <summary>
        /// Get the uuid field of the given PCI.
        /// First published in XenServer 6.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pci">The opaque_ref of the given pci</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_uuid(Session session, string _pci)
        {
            return session.JsonRpcClient.pci_get_uuid(session.opaque_ref, _pci);
        }

        /// <summary>
        /// Get the class_id field of the given PCI.
        /// First published in XenServer 6.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pci">The opaque_ref of the given pci</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_class_id(Session session, string _pci)
        {
            return session.JsonRpcClient.pci_get_class_id(session.opaque_ref, _pci);
        }

        /// <summary>
        /// Get the class_name field of the given PCI.
        /// First published in XenServer 6.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pci">The opaque_ref of the given pci</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_class_name(Session session, string _pci)
        {
            return session.JsonRpcClient.pci_get_class_name(session.opaque_ref, _pci);
        }

        /// <summary>
        /// Get the vendor_id field of the given PCI.
        /// First published in XenServer 6.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pci">The opaque_ref of the given pci</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_vendor_id(Session session, string _pci)
        {
            return session.JsonRpcClient.pci_get_vendor_id(session.opaque_ref, _pci);
        }

        /// <summary>
        /// Get the vendor_name field of the given PCI.
        /// First published in XenServer 6.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pci">The opaque_ref of the given pci</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_vendor_name(Session session, string _pci)
        {
            return session.JsonRpcClient.pci_get_vendor_name(session.opaque_ref, _pci);
        }

        /// <summary>
        /// Get the device_id field of the given PCI.
        /// First published in XenServer 6.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pci">The opaque_ref of the given pci</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_device_id(Session session, string _pci)
        {
            return session.JsonRpcClient.pci_get_device_id(session.opaque_ref, _pci);
        }

        /// <summary>
        /// Get the device_name field of the given PCI.
        /// First published in XenServer 6.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pci">The opaque_ref of the given pci</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_device_name(Session session, string _pci)
        {
            return session.JsonRpcClient.pci_get_device_name(session.opaque_ref, _pci);
        }

        /// <summary>
        /// Get the host field of the given PCI.
        /// First published in XenServer 6.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pci">The opaque_ref of the given pci</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static XenRef<Host> get_host(Session session, string _pci)
        {
            return session.JsonRpcClient.pci_get_host(session.opaque_ref, _pci);
        }

        /// <summary>
        /// Get the pci_id field of the given PCI.
        /// First published in XenServer 6.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pci">The opaque_ref of the given pci</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_pci_id(Session session, string _pci)
        {
            return session.JsonRpcClient.pci_get_pci_id(session.opaque_ref, _pci);
        }

        /// <summary>
        /// Get the dependencies field of the given PCI.
        /// First published in XenServer 6.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pci">The opaque_ref of the given pci</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static List<XenRef<PCI>> get_dependencies(Session session, string _pci)
        {
            return session.JsonRpcClient.pci_get_dependencies(session.opaque_ref, _pci);
        }

        /// <summary>
        /// Get the other_config field of the given PCI.
        /// First published in XenServer 6.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pci">The opaque_ref of the given pci</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static Dictionary<string, string> get_other_config(Session session, string _pci)
        {
            return session.JsonRpcClient.pci_get_other_config(session.opaque_ref, _pci);
        }

        /// <summary>
        /// Get the subsystem_vendor_id field of the given PCI.
        /// First published in XenServer 6.2 SP1 Hotfix 11.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pci">The opaque_ref of the given pci</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_subsystem_vendor_id(Session session, string _pci)
        {
            return session.JsonRpcClient.pci_get_subsystem_vendor_id(session.opaque_ref, _pci);
        }

        /// <summary>
        /// Get the subsystem_vendor_name field of the given PCI.
        /// First published in XenServer 6.2 SP1 Hotfix 11.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pci">The opaque_ref of the given pci</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_subsystem_vendor_name(Session session, string _pci)
        {
            return session.JsonRpcClient.pci_get_subsystem_vendor_name(session.opaque_ref, _pci);
        }

        /// <summary>
        /// Get the subsystem_device_id field of the given PCI.
        /// First published in XenServer 6.2 SP1 Hotfix 11.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pci">The opaque_ref of the given pci</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_subsystem_device_id(Session session, string _pci)
        {
            return session.JsonRpcClient.pci_get_subsystem_device_id(session.opaque_ref, _pci);
        }

        /// <summary>
        /// Get the subsystem_device_name field of the given PCI.
        /// First published in XenServer 6.2 SP1 Hotfix 11.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pci">The opaque_ref of the given pci</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_subsystem_device_name(Session session, string _pci)
        {
            return session.JsonRpcClient.pci_get_subsystem_device_name(session.opaque_ref, _pci);
        }

        /// <summary>
        /// Get the driver_name field of the given PCI.
        /// First published in XenServer 7.5.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pci">The opaque_ref of the given pci</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_driver_name(Session session, string _pci)
        {
            return session.JsonRpcClient.pci_get_driver_name(session.opaque_ref, _pci);
        }

        /// <summary>
        /// Set the other_config field of the given PCI.
        /// First published in XenServer 6.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pci">The opaque_ref of the given pci</param>
        /// <param name="_other_config">New value to set</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void set_other_config(Session session, string _pci, Dictionary<string, string> _other_config)
        {
            session.JsonRpcClient.pci_set_other_config(session.opaque_ref, _pci, _other_config);
        }

        /// <summary>
        /// Add the given key-value pair to the other_config field of the given PCI.
        /// First published in XenServer 6.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pci">The opaque_ref of the given pci</param>
        /// <param name="_key">Key to add</param>
        /// <param name="_value">Value to add</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void add_to_other_config(Session session, string _pci, string _key, string _value)
        {
            session.JsonRpcClient.pci_add_to_other_config(session.opaque_ref, _pci, _key, _value);
        }

        /// <summary>
        /// Remove the given key and its corresponding value from the other_config field of the given PCI.  If the key is not in that Map, then do nothing.
        /// First published in XenServer 6.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pci">The opaque_ref of the given pci</param>
        /// <param name="_key">Key to remove</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void remove_from_other_config(Session session, string _pci, string _key)
        {
            session.JsonRpcClient.pci_remove_from_other_config(session.opaque_ref, _pci, _key);
        }

        /// <summary>
        /// Hide a PCI device from the dom0 kernel. (Takes affect after next boot.)
        /// Experimental. First published in 24.14.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pci">The opaque_ref of the given pci</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static pci_dom0_access disable_dom0_access(Session session, string _pci)
        {
            return session.JsonRpcClient.pci_disable_dom0_access(session.opaque_ref, _pci);
        }

        /// <summary>
        /// Hide a PCI device from the dom0 kernel. (Takes affect after next boot.)
        /// Experimental. First published in 24.14.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pci">The opaque_ref of the given pci</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Task> async_disable_dom0_access(Session session, string _pci)
        {
          return session.JsonRpcClient.async_pci_disable_dom0_access(session.opaque_ref, _pci);
        }

        /// <summary>
        /// Unhide a PCI device from the dom0 kernel. (Takes affect after next boot.)
        /// Experimental. First published in 24.14.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pci">The opaque_ref of the given pci</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static pci_dom0_access enable_dom0_access(Session session, string _pci)
        {
            return session.JsonRpcClient.pci_enable_dom0_access(session.opaque_ref, _pci);
        }

        /// <summary>
        /// Unhide a PCI device from the dom0 kernel. (Takes affect after next boot.)
        /// Experimental. First published in 24.14.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pci">The opaque_ref of the given pci</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Task> async_enable_dom0_access(Session session, string _pci)
        {
          return session.JsonRpcClient.async_pci_enable_dom0_access(session.opaque_ref, _pci);
        }

        /// <summary>
        /// Return a PCI device dom0 access status.
        /// Experimental. First published in 24.14.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pci">The opaque_ref of the given pci</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static pci_dom0_access get_dom0_access_status(Session session, string _pci)
        {
            return session.JsonRpcClient.pci_get_dom0_access_status(session.opaque_ref, _pci);
        }

        /// <summary>
        /// Return a PCI device dom0 access status.
        /// Experimental. First published in 24.14.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pci">The opaque_ref of the given pci</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Task> async_get_dom0_access_status(Session session, string _pci)
        {
          return session.JsonRpcClient.async_pci_get_dom0_access_status(session.opaque_ref, _pci);
        }

        /// <summary>
        /// Return a list of all the PCIs known to the system.
        /// First published in XenServer 6.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static List<XenRef<PCI>> get_all(Session session)
        {
            return session.JsonRpcClient.pci_get_all(session.opaque_ref);
        }

        /// <summary>
        /// Return a map of PCI references to PCI records for all PCIs known to the system.
        /// First published in XenServer 6.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static Dictionary<XenRef<PCI>, PCI> get_all_records(Session session)
        {
            return session.JsonRpcClient.pci_get_all_records(session.opaque_ref);
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
        /// PCI class ID
        /// </summary>
        public virtual string class_id
        {
            get { return _class_id; }
            set
            {
                if (!Helper.AreEqual(value, _class_id))
                {
                    _class_id = value;
                    NotifyPropertyChanged("class_id");
                }
            }
        }
        private string _class_id = "";

        /// <summary>
        /// PCI class name
        /// </summary>
        public virtual string class_name
        {
            get { return _class_name; }
            set
            {
                if (!Helper.AreEqual(value, _class_name))
                {
                    _class_name = value;
                    NotifyPropertyChanged("class_name");
                }
            }
        }
        private string _class_name = "";

        /// <summary>
        /// Vendor ID
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
        /// Vendor name
        /// </summary>
        public virtual string vendor_name
        {
            get { return _vendor_name; }
            set
            {
                if (!Helper.AreEqual(value, _vendor_name))
                {
                    _vendor_name = value;
                    NotifyPropertyChanged("vendor_name");
                }
            }
        }
        private string _vendor_name = "";

        /// <summary>
        /// Device ID
        /// </summary>
        public virtual string device_id
        {
            get { return _device_id; }
            set
            {
                if (!Helper.AreEqual(value, _device_id))
                {
                    _device_id = value;
                    NotifyPropertyChanged("device_id");
                }
            }
        }
        private string _device_id = "";

        /// <summary>
        /// Device name
        /// </summary>
        public virtual string device_name
        {
            get { return _device_name; }
            set
            {
                if (!Helper.AreEqual(value, _device_name))
                {
                    _device_name = value;
                    NotifyPropertyChanged("device_name");
                }
            }
        }
        private string _device_name = "";

        /// <summary>
        /// Physical machine that owns the PCI device
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
        /// PCI ID of the physical device
        /// </summary>
        public virtual string pci_id
        {
            get { return _pci_id; }
            set
            {
                if (!Helper.AreEqual(value, _pci_id))
                {
                    _pci_id = value;
                    NotifyPropertyChanged("pci_id");
                }
            }
        }
        private string _pci_id = "";

        /// <summary>
        /// List of dependent PCI devices
        /// </summary>
        [JsonConverter(typeof(XenRefListConverter<PCI>))]
        public virtual List<XenRef<PCI>> dependencies
        {
            get { return _dependencies; }
            set
            {
                if (!Helper.AreEqual(value, _dependencies))
                {
                    _dependencies = value;
                    NotifyPropertyChanged("dependencies");
                }
            }
        }
        private List<XenRef<PCI>> _dependencies = new List<XenRef<PCI>>() {};

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
        /// Subsystem vendor ID
        /// First published in XenServer 6.2 SP1 Hotfix 11.
        /// </summary>
        public virtual string subsystem_vendor_id
        {
            get { return _subsystem_vendor_id; }
            set
            {
                if (!Helper.AreEqual(value, _subsystem_vendor_id))
                {
                    _subsystem_vendor_id = value;
                    NotifyPropertyChanged("subsystem_vendor_id");
                }
            }
        }
        private string _subsystem_vendor_id = "";

        /// <summary>
        /// Subsystem vendor name
        /// First published in XenServer 6.2 SP1 Hotfix 11.
        /// </summary>
        public virtual string subsystem_vendor_name
        {
            get { return _subsystem_vendor_name; }
            set
            {
                if (!Helper.AreEqual(value, _subsystem_vendor_name))
                {
                    _subsystem_vendor_name = value;
                    NotifyPropertyChanged("subsystem_vendor_name");
                }
            }
        }
        private string _subsystem_vendor_name = "";

        /// <summary>
        /// Subsystem device ID
        /// First published in XenServer 6.2 SP1 Hotfix 11.
        /// </summary>
        public virtual string subsystem_device_id
        {
            get { return _subsystem_device_id; }
            set
            {
                if (!Helper.AreEqual(value, _subsystem_device_id))
                {
                    _subsystem_device_id = value;
                    NotifyPropertyChanged("subsystem_device_id");
                }
            }
        }
        private string _subsystem_device_id = "";

        /// <summary>
        /// Subsystem device name
        /// First published in XenServer 6.2 SP1 Hotfix 11.
        /// </summary>
        public virtual string subsystem_device_name
        {
            get { return _subsystem_device_name; }
            set
            {
                if (!Helper.AreEqual(value, _subsystem_device_name))
                {
                    _subsystem_device_name = value;
                    NotifyPropertyChanged("subsystem_device_name");
                }
            }
        }
        private string _subsystem_device_name = "";

        /// <summary>
        /// Driver name
        /// First published in XenServer 7.5.
        /// </summary>
        public virtual string driver_name
        {
            get { return _driver_name; }
            set
            {
                if (!Helper.AreEqual(value, _driver_name))
                {
                    _driver_name = value;
                    NotifyPropertyChanged("driver_name");
                }
            }
        }
        private string _driver_name = "";
    }
}
