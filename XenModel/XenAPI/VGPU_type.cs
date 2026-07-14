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
    /// A type of virtual GPU
    /// First published in XenServer 6.2 SP1 Tech-Preview.
    /// </summary>
    public partial class VGPU_type : XenObject<VGPU_type>
    {
        #region Constructors

        public VGPU_type()
        {
        }

        public VGPU_type(string uuid,
            string vendor_name,
            string model_name,
            long framebuffer_size,
            long max_heads,
            long max_resolution_x,
            long max_resolution_y,
            List<XenRef<PGPU>> supported_on_PGPUs,
            List<XenRef<PGPU>> enabled_on_PGPUs,
            List<XenRef<VGPU>> VGPUs,
            List<XenRef<GPU_group>> supported_on_GPU_groups,
            List<XenRef<GPU_group>> enabled_on_GPU_groups,
            vgpu_type_implementation implementation,
            string identifier,
            bool experimental,
            List<XenRef<VGPU_type>> compatible_types_in_vm)
        {
            this.uuid = uuid;
            this.vendor_name = vendor_name;
            this.model_name = model_name;
            this.framebuffer_size = framebuffer_size;
            this.max_heads = max_heads;
            this.max_resolution_x = max_resolution_x;
            this.max_resolution_y = max_resolution_y;
            this.supported_on_PGPUs = supported_on_PGPUs;
            this.enabled_on_PGPUs = enabled_on_PGPUs;
            this.VGPUs = VGPUs;
            this.supported_on_GPU_groups = supported_on_GPU_groups;
            this.enabled_on_GPU_groups = enabled_on_GPU_groups;
            this.implementation = implementation;
            this.identifier = identifier;
            this.experimental = experimental;
            this.compatible_types_in_vm = compatible_types_in_vm;
        }

        /// <summary>
        /// Creates a new VGPU_type from a Hashtable.
        /// Note that the fields not contained in the Hashtable
        /// will be created with their default values.
        /// </summary>
        /// <param name="table"></param>
        public VGPU_type(Hashtable table)
            : this()
        {
            UpdateFrom(table);
        }

        #endregion

        /// <summary>
        /// Updates each field of this instance with the value of
        /// the corresponding field of a given VGPU_type.
        /// </summary>
        public override void UpdateFrom(VGPU_type record)
        {
            uuid = record.uuid;
            vendor_name = record.vendor_name;
            model_name = record.model_name;
            framebuffer_size = record.framebuffer_size;
            max_heads = record.max_heads;
            max_resolution_x = record.max_resolution_x;
            max_resolution_y = record.max_resolution_y;
            supported_on_PGPUs = record.supported_on_PGPUs;
            enabled_on_PGPUs = record.enabled_on_PGPUs;
            VGPUs = record.VGPUs;
            supported_on_GPU_groups = record.supported_on_GPU_groups;
            enabled_on_GPU_groups = record.enabled_on_GPU_groups;
            implementation = record.implementation;
            identifier = record.identifier;
            experimental = record.experimental;
            compatible_types_in_vm = record.compatible_types_in_vm;
        }

        /// <summary>
        /// Given a Hashtable with field-value pairs, it updates the fields of this VGPU_type
        /// with the values listed in the Hashtable. Note that only the fields contained
        /// in the Hashtable will be updated and the rest will remain the same.
        /// </summary>
        /// <param name="table"></param>
        public void UpdateFrom(Hashtable table)
        {
            if (table.ContainsKey("uuid"))
                uuid = Marshalling.ParseString(table, "uuid");
            if (table.ContainsKey("vendor_name"))
                vendor_name = Marshalling.ParseString(table, "vendor_name");
            if (table.ContainsKey("model_name"))
                model_name = Marshalling.ParseString(table, "model_name");
            if (table.ContainsKey("framebuffer_size"))
                framebuffer_size = Marshalling.ParseLong(table, "framebuffer_size");
            if (table.ContainsKey("max_heads"))
                max_heads = Marshalling.ParseLong(table, "max_heads");
            if (table.ContainsKey("max_resolution_x"))
                max_resolution_x = Marshalling.ParseLong(table, "max_resolution_x");
            if (table.ContainsKey("max_resolution_y"))
                max_resolution_y = Marshalling.ParseLong(table, "max_resolution_y");
            if (table.ContainsKey("supported_on_PGPUs"))
                supported_on_PGPUs = Marshalling.ParseSetRef<PGPU>(table, "supported_on_PGPUs");
            if (table.ContainsKey("enabled_on_PGPUs"))
                enabled_on_PGPUs = Marshalling.ParseSetRef<PGPU>(table, "enabled_on_PGPUs");
            if (table.ContainsKey("VGPUs"))
                VGPUs = Marshalling.ParseSetRef<VGPU>(table, "VGPUs");
            if (table.ContainsKey("supported_on_GPU_groups"))
                supported_on_GPU_groups = Marshalling.ParseSetRef<GPU_group>(table, "supported_on_GPU_groups");
            if (table.ContainsKey("enabled_on_GPU_groups"))
                enabled_on_GPU_groups = Marshalling.ParseSetRef<GPU_group>(table, "enabled_on_GPU_groups");
            if (table.ContainsKey("implementation"))
                implementation = (vgpu_type_implementation)Helper.EnumParseDefault(typeof(vgpu_type_implementation), Marshalling.ParseString(table, "implementation"));
            if (table.ContainsKey("identifier"))
                identifier = Marshalling.ParseString(table, "identifier");
            if (table.ContainsKey("experimental"))
                experimental = Marshalling.ParseBool(table, "experimental");
            if (table.ContainsKey("compatible_types_in_vm"))
                compatible_types_in_vm = Marshalling.ParseSetRef<VGPU_type>(table, "compatible_types_in_vm");
        }

        public bool DeepEquals(VGPU_type other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;

            return Helper.AreEqual2(_uuid, other._uuid) &&
                Helper.AreEqual2(_vendor_name, other._vendor_name) &&
                Helper.AreEqual2(_model_name, other._model_name) &&
                Helper.AreEqual2(_framebuffer_size, other._framebuffer_size) &&
                Helper.AreEqual2(_max_heads, other._max_heads) &&
                Helper.AreEqual2(_max_resolution_x, other._max_resolution_x) &&
                Helper.AreEqual2(_max_resolution_y, other._max_resolution_y) &&
                Helper.AreEqual2(_supported_on_PGPUs, other._supported_on_PGPUs) &&
                Helper.AreEqual2(_enabled_on_PGPUs, other._enabled_on_PGPUs) &&
                Helper.AreEqual2(_VGPUs, other._VGPUs) &&
                Helper.AreEqual2(_supported_on_GPU_groups, other._supported_on_GPU_groups) &&
                Helper.AreEqual2(_enabled_on_GPU_groups, other._enabled_on_GPU_groups) &&
                Helper.AreEqual2(_implementation, other._implementation) &&
                Helper.AreEqual2(_identifier, other._identifier) &&
                Helper.AreEqual2(_experimental, other._experimental) &&
                Helper.AreEqual2(_compatible_types_in_vm, other._compatible_types_in_vm);
        }


        /// <summary>
        /// Get a record containing the current state of the given VGPU_type.
        /// First published in XenServer 6.2 SP1 Tech-Preview.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vgpu_type">The opaque_ref of the given vgpu_type</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static VGPU_type get_record(Session session, string _vgpu_type)
        {
            return session.JsonRpcClient.vgpu_type_get_record(session.opaque_ref, _vgpu_type);
        }

        /// <summary>
        /// Get a reference to the VGPU_type instance with the specified UUID.
        /// First published in XenServer 6.2 SP1 Tech-Preview.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_uuid">UUID of object to return</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static XenRef<VGPU_type> get_by_uuid(Session session, string _uuid)
        {
            return session.JsonRpcClient.vgpu_type_get_by_uuid(session.opaque_ref, _uuid);
        }

        /// <summary>
        /// Get the uuid field of the given VGPU_type.
        /// First published in XenServer 6.2 SP1 Tech-Preview.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vgpu_type">The opaque_ref of the given vgpu_type</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_uuid(Session session, string _vgpu_type)
        {
            return session.JsonRpcClient.vgpu_type_get_uuid(session.opaque_ref, _vgpu_type);
        }

        /// <summary>
        /// Get the vendor_name field of the given VGPU_type.
        /// First published in XenServer 6.2 SP1 Tech-Preview.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vgpu_type">The opaque_ref of the given vgpu_type</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_vendor_name(Session session, string _vgpu_type)
        {
            return session.JsonRpcClient.vgpu_type_get_vendor_name(session.opaque_ref, _vgpu_type);
        }

        /// <summary>
        /// Get the model_name field of the given VGPU_type.
        /// First published in XenServer 6.2 SP1 Tech-Preview.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vgpu_type">The opaque_ref of the given vgpu_type</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_model_name(Session session, string _vgpu_type)
        {
            return session.JsonRpcClient.vgpu_type_get_model_name(session.opaque_ref, _vgpu_type);
        }

        /// <summary>
        /// Get the framebuffer_size field of the given VGPU_type.
        /// First published in XenServer 6.2 SP1 Tech-Preview.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vgpu_type">The opaque_ref of the given vgpu_type</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static long get_framebuffer_size(Session session, string _vgpu_type)
        {
            return session.JsonRpcClient.vgpu_type_get_framebuffer_size(session.opaque_ref, _vgpu_type);
        }

        /// <summary>
        /// Get the max_heads field of the given VGPU_type.
        /// First published in XenServer 6.2 SP1 Tech-Preview.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vgpu_type">The opaque_ref of the given vgpu_type</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static long get_max_heads(Session session, string _vgpu_type)
        {
            return session.JsonRpcClient.vgpu_type_get_max_heads(session.opaque_ref, _vgpu_type);
        }

        /// <summary>
        /// Get the max_resolution_x field of the given VGPU_type.
        /// First published in XenServer 6.2 SP1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vgpu_type">The opaque_ref of the given vgpu_type</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static long get_max_resolution_x(Session session, string _vgpu_type)
        {
            return session.JsonRpcClient.vgpu_type_get_max_resolution_x(session.opaque_ref, _vgpu_type);
        }

        /// <summary>
        /// Get the max_resolution_y field of the given VGPU_type.
        /// First published in XenServer 6.2 SP1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vgpu_type">The opaque_ref of the given vgpu_type</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static long get_max_resolution_y(Session session, string _vgpu_type)
        {
            return session.JsonRpcClient.vgpu_type_get_max_resolution_y(session.opaque_ref, _vgpu_type);
        }

        /// <summary>
        /// Get the supported_on_PGPUs field of the given VGPU_type.
        /// First published in XenServer 6.2 SP1 Tech-Preview.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vgpu_type">The opaque_ref of the given vgpu_type</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static List<XenRef<PGPU>> get_supported_on_PGPUs(Session session, string _vgpu_type)
        {
            return session.JsonRpcClient.vgpu_type_get_supported_on_pgpus(session.opaque_ref, _vgpu_type);
        }

        /// <summary>
        /// Get the enabled_on_PGPUs field of the given VGPU_type.
        /// First published in XenServer 6.2 SP1 Tech-Preview.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vgpu_type">The opaque_ref of the given vgpu_type</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static List<XenRef<PGPU>> get_enabled_on_PGPUs(Session session, string _vgpu_type)
        {
            return session.JsonRpcClient.vgpu_type_get_enabled_on_pgpus(session.opaque_ref, _vgpu_type);
        }

        /// <summary>
        /// Get the VGPUs field of the given VGPU_type.
        /// First published in XenServer 6.2 SP1 Tech-Preview.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vgpu_type">The opaque_ref of the given vgpu_type</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static List<XenRef<VGPU>> get_VGPUs(Session session, string _vgpu_type)
        {
            return session.JsonRpcClient.vgpu_type_get_vgpus(session.opaque_ref, _vgpu_type);
        }

        /// <summary>
        /// Get the supported_on_GPU_groups field of the given VGPU_type.
        /// First published in XenServer 6.2 SP1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vgpu_type">The opaque_ref of the given vgpu_type</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static List<XenRef<GPU_group>> get_supported_on_GPU_groups(Session session, string _vgpu_type)
        {
            return session.JsonRpcClient.vgpu_type_get_supported_on_gpu_groups(session.opaque_ref, _vgpu_type);
        }

        /// <summary>
        /// Get the enabled_on_GPU_groups field of the given VGPU_type.
        /// First published in XenServer 6.2 SP1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vgpu_type">The opaque_ref of the given vgpu_type</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static List<XenRef<GPU_group>> get_enabled_on_GPU_groups(Session session, string _vgpu_type)
        {
            return session.JsonRpcClient.vgpu_type_get_enabled_on_gpu_groups(session.opaque_ref, _vgpu_type);
        }

        /// <summary>
        /// Get the implementation field of the given VGPU_type.
        /// First published in XenServer 7.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vgpu_type">The opaque_ref of the given vgpu_type</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static vgpu_type_implementation get_implementation(Session session, string _vgpu_type)
        {
            return session.JsonRpcClient.vgpu_type_get_implementation(session.opaque_ref, _vgpu_type);
        }

        /// <summary>
        /// Get the identifier field of the given VGPU_type.
        /// First published in XenServer 7.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vgpu_type">The opaque_ref of the given vgpu_type</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_identifier(Session session, string _vgpu_type)
        {
            return session.JsonRpcClient.vgpu_type_get_identifier(session.opaque_ref, _vgpu_type);
        }

        /// <summary>
        /// Get the experimental field of the given VGPU_type.
        /// First published in XenServer 7.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vgpu_type">The opaque_ref of the given vgpu_type</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static bool get_experimental(Session session, string _vgpu_type)
        {
            return session.JsonRpcClient.vgpu_type_get_experimental(session.opaque_ref, _vgpu_type);
        }

        /// <summary>
        /// Get the compatible_types_in_vm field of the given VGPU_type.
        /// First published in Citrix Hypervisor 8.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vgpu_type">The opaque_ref of the given vgpu_type</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static List<XenRef<VGPU_type>> get_compatible_types_in_vm(Session session, string _vgpu_type)
        {
            return session.JsonRpcClient.vgpu_type_get_compatible_types_in_vm(session.opaque_ref, _vgpu_type);
        }

        /// <summary>
        /// Return a list of all the VGPU_types known to the system.
        /// First published in XenServer 6.2 SP1 Tech-Preview.
        /// </summary>
        /// <param name="session">The session</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static List<XenRef<VGPU_type>> get_all(Session session)
        {
            return session.JsonRpcClient.vgpu_type_get_all(session.opaque_ref);
        }

        /// <summary>
        /// Return a map of VGPU_type references to VGPU_type records for all VGPU_types known to the system.
        /// First published in XenServer 6.2 SP1 Tech-Preview.
        /// </summary>
        /// <param name="session">The session</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static Dictionary<XenRef<VGPU_type>, VGPU_type> get_all_records(Session session)
        {
            return session.JsonRpcClient.vgpu_type_get_all_records(session.opaque_ref);
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
        /// Name of VGPU vendor
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
        /// Model name associated with the VGPU type
        /// </summary>
        public virtual string model_name
        {
            get { return _model_name; }
            set
            {
                if (!Helper.AreEqual(value, _model_name))
                {
                    _model_name = value;
                    NotifyPropertyChanged("model_name");
                }
            }
        }
        private string _model_name = "";

        /// <summary>
        /// Framebuffer size of the VGPU type, in bytes
        /// </summary>
        public virtual long framebuffer_size
        {
            get { return _framebuffer_size; }
            set
            {
                if (!Helper.AreEqual(value, _framebuffer_size))
                {
                    _framebuffer_size = value;
                    NotifyPropertyChanged("framebuffer_size");
                }
            }
        }
        private long _framebuffer_size = 0;

        /// <summary>
        /// Maximum number of displays supported by the VGPU type
        /// </summary>
        public virtual long max_heads
        {
            get { return _max_heads; }
            set
            {
                if (!Helper.AreEqual(value, _max_heads))
                {
                    _max_heads = value;
                    NotifyPropertyChanged("max_heads");
                }
            }
        }
        private long _max_heads = 0;

        /// <summary>
        /// Maximum resolution (width) supported by the VGPU type
        /// First published in XenServer 6.2 SP1.
        /// </summary>
        public virtual long max_resolution_x
        {
            get { return _max_resolution_x; }
            set
            {
                if (!Helper.AreEqual(value, _max_resolution_x))
                {
                    _max_resolution_x = value;
                    NotifyPropertyChanged("max_resolution_x");
                }
            }
        }
        private long _max_resolution_x = 0;

        /// <summary>
        /// Maximum resolution (height) supported by the VGPU type
        /// First published in XenServer 6.2 SP1.
        /// </summary>
        public virtual long max_resolution_y
        {
            get { return _max_resolution_y; }
            set
            {
                if (!Helper.AreEqual(value, _max_resolution_y))
                {
                    _max_resolution_y = value;
                    NotifyPropertyChanged("max_resolution_y");
                }
            }
        }
        private long _max_resolution_y = 0;

        /// <summary>
        /// List of PGPUs that support this VGPU type
        /// </summary>
        [JsonConverter(typeof(XenRefListConverter<PGPU>))]
        public virtual List<XenRef<PGPU>> supported_on_PGPUs
        {
            get { return _supported_on_PGPUs; }
            set
            {
                if (!Helper.AreEqual(value, _supported_on_PGPUs))
                {
                    _supported_on_PGPUs = value;
                    NotifyPropertyChanged("supported_on_PGPUs");
                }
            }
        }
        private List<XenRef<PGPU>> _supported_on_PGPUs = new List<XenRef<PGPU>>() {};

        /// <summary>
        /// List of PGPUs that have this VGPU type enabled
        /// </summary>
        [JsonConverter(typeof(XenRefListConverter<PGPU>))]
        public virtual List<XenRef<PGPU>> enabled_on_PGPUs
        {
            get { return _enabled_on_PGPUs; }
            set
            {
                if (!Helper.AreEqual(value, _enabled_on_PGPUs))
                {
                    _enabled_on_PGPUs = value;
                    NotifyPropertyChanged("enabled_on_PGPUs");
                }
            }
        }
        private List<XenRef<PGPU>> _enabled_on_PGPUs = new List<XenRef<PGPU>>() {};

        /// <summary>
        /// List of VGPUs of this type
        /// </summary>
        [JsonConverter(typeof(XenRefListConverter<VGPU>))]
        public virtual List<XenRef<VGPU>> VGPUs
        {
            get { return _VGPUs; }
            set
            {
                if (!Helper.AreEqual(value, _VGPUs))
                {
                    _VGPUs = value;
                    NotifyPropertyChanged("VGPUs");
                }
            }
        }
        private List<XenRef<VGPU>> _VGPUs = new List<XenRef<VGPU>>() {};

        /// <summary>
        /// List of GPU groups in which at least one PGPU supports this VGPU type
        /// First published in XenServer 6.2 SP1.
        /// </summary>
        [JsonConverter(typeof(XenRefListConverter<GPU_group>))]
        public virtual List<XenRef<GPU_group>> supported_on_GPU_groups
        {
            get { return _supported_on_GPU_groups; }
            set
            {
                if (!Helper.AreEqual(value, _supported_on_GPU_groups))
                {
                    _supported_on_GPU_groups = value;
                    NotifyPropertyChanged("supported_on_GPU_groups");
                }
            }
        }
        private List<XenRef<GPU_group>> _supported_on_GPU_groups = new List<XenRef<GPU_group>>() {};

        /// <summary>
        /// List of GPU groups in which at least one have this VGPU type enabled
        /// First published in XenServer 6.2 SP1.
        /// </summary>
        [JsonConverter(typeof(XenRefListConverter<GPU_group>))]
        public virtual List<XenRef<GPU_group>> enabled_on_GPU_groups
        {
            get { return _enabled_on_GPU_groups; }
            set
            {
                if (!Helper.AreEqual(value, _enabled_on_GPU_groups))
                {
                    _enabled_on_GPU_groups = value;
                    NotifyPropertyChanged("enabled_on_GPU_groups");
                }
            }
        }
        private List<XenRef<GPU_group>> _enabled_on_GPU_groups = new List<XenRef<GPU_group>>() {};

        /// <summary>
        /// The internal implementation of this VGPU type
        /// First published in XenServer 7.0.
        /// </summary>
        [JsonConverter(typeof(vgpu_type_implementationConverter))]
        public virtual vgpu_type_implementation implementation
        {
            get { return _implementation; }
            set
            {
                if (!Helper.AreEqual(value, _implementation))
                {
                    _implementation = value;
                    NotifyPropertyChanged("implementation");
                }
            }
        }
        private vgpu_type_implementation _implementation = vgpu_type_implementation.passthrough;

        /// <summary>
        /// Key used to identify VGPU types and avoid creating duplicates - this field is used internally and not intended for interpretation by API clients
        /// First published in XenServer 7.0.
        /// </summary>
        public virtual string identifier
        {
            get { return _identifier; }
            set
            {
                if (!Helper.AreEqual(value, _identifier))
                {
                    _identifier = value;
                    NotifyPropertyChanged("identifier");
                }
            }
        }
        private string _identifier = "";

        /// <summary>
        /// Indicates whether VGPUs of this type should be considered experimental
        /// First published in XenServer 7.0.
        /// </summary>
        public virtual bool experimental
        {
            get { return _experimental; }
            set
            {
                if (!Helper.AreEqual(value, _experimental))
                {
                    _experimental = value;
                    NotifyPropertyChanged("experimental");
                }
            }
        }
        private bool _experimental = false;

        /// <summary>
        /// List of VGPU types which are compatible in one VM
        /// First published in Citrix Hypervisor 8.1.
        /// </summary>
        [JsonConverter(typeof(XenRefListConverter<VGPU_type>))]
        public virtual List<XenRef<VGPU_type>> compatible_types_in_vm
        {
            get { return _compatible_types_in_vm; }
            set
            {
                if (!Helper.AreEqual(value, _compatible_types_in_vm))
                {
                    _compatible_types_in_vm = value;
                    NotifyPropertyChanged("compatible_types_in_vm");
                }
            }
        }
        private List<XenRef<VGPU_type>> _compatible_types_in_vm = new List<XenRef<VGPU_type>>() {};
    }
}
