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
    /// A VM group
    /// First published in .
    /// </summary>
    public partial class VM_group : XenObject<VM_group>
    {
        #region Constructors

        public VM_group()
        {
        }

        public VM_group(string uuid,
            string name_label,
            string name_description,
            placement_policy placement,
            List<XenRef<VM>> VMs)
        {
            this.uuid = uuid;
            this.name_label = name_label;
            this.name_description = name_description;
            this.placement = placement;
            this.VMs = VMs;
        }

        /// <summary>
        /// Creates a new VM_group from a Hashtable.
        /// Note that the fields not contained in the Hashtable
        /// will be created with their default values.
        /// </summary>
        /// <param name="table"></param>
        public VM_group(Hashtable table)
            : this()
        {
            UpdateFrom(table);
        }

        #endregion

        /// <summary>
        /// Updates each field of this instance with the value of
        /// the corresponding field of a given VM_group.
        /// </summary>
        public override void UpdateFrom(VM_group record)
        {
            uuid = record.uuid;
            name_label = record.name_label;
            name_description = record.name_description;
            placement = record.placement;
            VMs = record.VMs;
        }

        /// <summary>
        /// Given a Hashtable with field-value pairs, it updates the fields of this VM_group
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
            if (table.ContainsKey("placement"))
                placement = (placement_policy)Helper.EnumParseDefault(typeof(placement_policy), Marshalling.ParseString(table, "placement"));
            if (table.ContainsKey("VMs"))
                VMs = Marshalling.ParseSetRef<VM>(table, "VMs");
        }

        public bool DeepEquals(VM_group other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;

            return Helper.AreEqual2(_uuid, other._uuid) &&
                Helper.AreEqual2(_name_label, other._name_label) &&
                Helper.AreEqual2(_name_description, other._name_description) &&
                Helper.AreEqual2(_placement, other._placement) &&
                Helper.AreEqual2(_VMs, other._VMs);
        }


        /// <summary>
        /// Get a record containing the current state of the given VM_group.
        /// Experimental. First published in 24.19.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm_group">The opaque_ref of the given vm_group</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static VM_group get_record(Session session, string _vm_group)
        {
            return session.JsonRpcClient.vm_group_get_record(session.opaque_ref, _vm_group);
        }

        /// <summary>
        /// Get a reference to the VM_group instance with the specified UUID.
        /// Experimental. First published in 24.19.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_uuid">UUID of object to return</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static XenRef<VM_group> get_by_uuid(Session session, string _uuid)
        {
            return session.JsonRpcClient.vm_group_get_by_uuid(session.opaque_ref, _uuid);
        }

        /// <summary>
        /// Create a new VM_group instance, and return its handle.
        /// Experimental. First published in 24.19.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_record">All constructor arguments</param>
        /// <remarks>
        /// Minimum allowed role: vm-admin
        /// </remarks>
        public static XenRef<VM_group> create(Session session, VM_group _record)
        {
            return session.JsonRpcClient.vm_group_create(session.opaque_ref, _record);
        }

        /// <summary>
        /// Create a new VM_group instance, and return its handle.
        /// Experimental. First published in 24.19.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_record">All constructor arguments</param>
        /// <remarks>
        /// Minimum allowed role: vm-admin
        /// </remarks>
        public static XenRef<Task> async_create(Session session, VM_group _record)
        {
          return session.JsonRpcClient.async_vm_group_create(session.opaque_ref, _record);
        }

        /// <summary>
        /// Destroy the specified VM_group instance.
        /// Experimental. First published in 24.19.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm_group">The opaque_ref of the given vm_group</param>
        /// <remarks>
        /// Minimum allowed role: vm-admin
        /// </remarks>
        public static void destroy(Session session, string _vm_group)
        {
            session.JsonRpcClient.vm_group_destroy(session.opaque_ref, _vm_group);
        }

        /// <summary>
        /// Destroy the specified VM_group instance.
        /// Experimental. First published in 24.19.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm_group">The opaque_ref of the given vm_group</param>
        /// <remarks>
        /// Minimum allowed role: vm-admin
        /// </remarks>
        public static XenRef<Task> async_destroy(Session session, string _vm_group)
        {
          return session.JsonRpcClient.async_vm_group_destroy(session.opaque_ref, _vm_group);
        }

        /// <summary>
        /// Get all the VM_group instances with the given label.
        /// Experimental. First published in 24.19.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_label">label of object to return</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static List<XenRef<VM_group>> get_by_name_label(Session session, string _label)
        {
            return session.JsonRpcClient.vm_group_get_by_name_label(session.opaque_ref, _label);
        }

        /// <summary>
        /// Get the uuid field of the given VM_group.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm_group">The opaque_ref of the given vm_group</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_uuid(Session session, string _vm_group)
        {
            return session.JsonRpcClient.vm_group_get_uuid(session.opaque_ref, _vm_group);
        }

        /// <summary>
        /// Get the name/label field of the given VM_group.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm_group">The opaque_ref of the given vm_group</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_name_label(Session session, string _vm_group)
        {
            return session.JsonRpcClient.vm_group_get_name_label(session.opaque_ref, _vm_group);
        }

        /// <summary>
        /// Get the name/description field of the given VM_group.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm_group">The opaque_ref of the given vm_group</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_name_description(Session session, string _vm_group)
        {
            return session.JsonRpcClient.vm_group_get_name_description(session.opaque_ref, _vm_group);
        }

        /// <summary>
        /// Get the placement field of the given VM_group.
        /// Experimental. First published in 24.19.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm_group">The opaque_ref of the given vm_group</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static placement_policy get_placement(Session session, string _vm_group)
        {
            return session.JsonRpcClient.vm_group_get_placement(session.opaque_ref, _vm_group);
        }

        /// <summary>
        /// Get the VMs field of the given VM_group.
        /// Experimental. First published in 24.19.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm_group">The opaque_ref of the given vm_group</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static List<XenRef<VM>> get_VMs(Session session, string _vm_group)
        {
            return session.JsonRpcClient.vm_group_get_vms(session.opaque_ref, _vm_group);
        }

        /// <summary>
        /// Set the name/label field of the given VM_group.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm_group">The opaque_ref of the given vm_group</param>
        /// <param name="_label">New value to set</param>
        /// <remarks>
        /// Minimum allowed role: vm-admin
        /// </remarks>
        public static void set_name_label(Session session, string _vm_group, string _label)
        {
            session.JsonRpcClient.vm_group_set_name_label(session.opaque_ref, _vm_group, _label);
        }

        /// <summary>
        /// Set the name/description field of the given VM_group.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vm_group">The opaque_ref of the given vm_group</param>
        /// <param name="_description">New value to set</param>
        /// <remarks>
        /// Minimum allowed role: vm-admin
        /// </remarks>
        public static void set_name_description(Session session, string _vm_group, string _description)
        {
            session.JsonRpcClient.vm_group_set_name_description(session.opaque_ref, _vm_group, _description);
        }

        /// <summary>
        /// Return a list of all the VM_groups known to the system.
        /// Experimental. First published in 24.19.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static List<XenRef<VM_group>> get_all(Session session)
        {
            return session.JsonRpcClient.vm_group_get_all(session.opaque_ref);
        }

        /// <summary>
        /// Return a map of VM_group references to VM_group records for all VM_groups known to the system.
        /// Experimental. First published in 24.19.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static Dictionary<XenRef<VM_group>, VM_group> get_all_records(Session session)
        {
            return session.JsonRpcClient.vm_group_get_all_records(session.opaque_ref);
        }

        /// <summary>
        /// Unique identifier/object reference
        /// First published in XenServer 4.0.
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
        /// First published in XenServer 4.0.
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
        /// First published in XenServer 4.0.
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
        /// The placement policy of the VM group
        /// Experimental. First published in 24.19.1.
        /// </summary>
        [JsonConverter(typeof(placement_policyConverter))]
        public virtual placement_policy placement
        {
            get { return _placement; }
            set
            {
                if (!Helper.AreEqual(value, _placement))
                {
                    _placement = value;
                    NotifyPropertyChanged("placement");
                }
            }
        }
        private placement_policy _placement = placement_policy.normal;

        /// <summary>
        /// The list of VMs associated with the group
        /// Experimental. First published in 24.19.1.
        /// </summary>
        [JsonConverter(typeof(XenRefListConverter<VM>))]
        public virtual List<XenRef<VM>> VMs
        {
            get { return _VMs; }
            set
            {
                if (!Helper.AreEqual(value, _VMs))
                {
                    _VMs = value;
                    NotifyPropertyChanged("VMs");
                }
            }
        }
        private List<XenRef<VM>> _VMs = new List<XenRef<VM>>() {};
    }
}
