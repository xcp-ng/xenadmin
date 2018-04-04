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
using Newtonsoft.Json.Converters;


namespace XenAPI
{
    /// <summary>
    /// A placeholder for a binary blob
    /// First published in XenServer 5.0.
    /// </summary>
    public partial class Blob : XenObject<Blob>
    {
        public Blob()
        {
        }

        public Blob(string uuid,
            string name_label,
            string name_description,
            long size,
            bool pubblic,
            DateTime last_updated,
            string mime_type)
        {
            this.uuid = uuid;
            this.name_label = name_label;
            this.name_description = name_description;
            this.size = size;
            this.pubblic = pubblic;
            this.last_updated = last_updated;
            this.mime_type = mime_type;
        }

        /// <summary>
        /// Creates a new Blob from a Proxy_Blob.
        /// </summary>
        /// <param name="proxy"></param>
        public Blob(Proxy_Blob proxy)
        {
            this.UpdateFromProxy(proxy);
        }

        /// <summary>
        /// Updates each field of this instance with the value of
        /// the corresponding field of a given Blob.
        /// </summary>
        public override void UpdateFrom(Blob update)
        {
            uuid = update.uuid;
            name_label = update.name_label;
            name_description = update.name_description;
            size = update.size;
            pubblic = update.pubblic;
            last_updated = update.last_updated;
            mime_type = update.mime_type;
        }

        internal void UpdateFromProxy(Proxy_Blob proxy)
        {
            uuid = proxy.uuid == null ? null : proxy.uuid;
            name_label = proxy.name_label == null ? null : proxy.name_label;
            name_description = proxy.name_description == null ? null : proxy.name_description;
            size = proxy.size == null ? 0 : long.Parse(proxy.size);
            pubblic = (bool)proxy.pubblic;
            last_updated = proxy.last_updated;
            mime_type = proxy.mime_type == null ? null : proxy.mime_type;
        }

        public Proxy_Blob ToProxy()
        {
            Proxy_Blob result_ = new Proxy_Blob();
            result_.uuid = uuid ?? "";
            result_.name_label = name_label ?? "";
            result_.name_description = name_description ?? "";
            result_.size = size.ToString();
            result_.pubblic = pubblic;
            result_.last_updated = last_updated;
            result_.mime_type = mime_type ?? "";
            return result_;
        }

        /// <summary>
        /// Creates a new Blob from a Hashtable.
        /// Note that the fields not contained in the Hashtable
        /// will be created with their default values.
        /// </summary>
        /// <param name="table"></param>
        public Blob(Hashtable table) : this()
        {
            UpdateFrom(table);
        }

        /// <summary>
        /// Given a Hashtable with field-value pairs, it updates the fields of this Blob
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
            if (table.ContainsKey("size"))
                size = Marshalling.ParseLong(table, "size");
            if (table.ContainsKey("pubblic"))
                pubblic = Marshalling.ParseBool(table, "pubblic");
            if (table.ContainsKey("last_updated"))
                last_updated = Marshalling.ParseDateTime(table, "last_updated");
            if (table.ContainsKey("mime_type"))
                mime_type = Marshalling.ParseString(table, "mime_type");
        }

        public bool DeepEquals(Blob other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;

            return Helper.AreEqual2(this._uuid, other._uuid) &&
                Helper.AreEqual2(this._name_label, other._name_label) &&
                Helper.AreEqual2(this._name_description, other._name_description) &&
                Helper.AreEqual2(this._size, other._size) &&
                Helper.AreEqual2(this._pubblic, other._pubblic) &&
                Helper.AreEqual2(this._last_updated, other._last_updated) &&
                Helper.AreEqual2(this._mime_type, other._mime_type);
        }

        internal static List<Blob> ProxyArrayToObjectList(Proxy_Blob[] input)
        {
            var result = new List<Blob>();
            foreach (var item in input)
                result.Add(new Blob(item));

            return result;
        }

        public override string SaveChanges(Session session, string opaqueRef, Blob server)
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
                    Blob.set_name_label(session, opaqueRef, _name_label);
                }
                if (!Helper.AreEqual2(_name_description, server._name_description))
                {
                    Blob.set_name_description(session, opaqueRef, _name_description);
                }

                return null;
            }
        }
        /// <summary>
        /// Get a record containing the current state of the given blob.
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_blob">The opaque_ref of the given blob</param>
        public static Blob get_record(Session session, string _blob)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.blob_get_record(session.opaque_ref, _blob);
            else
                return new Blob((Proxy_Blob)session.proxy.blob_get_record(session.opaque_ref, _blob ?? "").parse());
        }

        /// <summary>
        /// Get a reference to the blob instance with the specified UUID.
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_uuid">UUID of object to return</param>
        public static XenRef<Blob> get_by_uuid(Session session, string _uuid)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.blob_get_by_uuid(session.opaque_ref, _uuid);
            else
                return XenRef<Blob>.Create(session.proxy.blob_get_by_uuid(session.opaque_ref, _uuid ?? "").parse());
        }

        /// <summary>
        /// Get all the blob instances with the given label.
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_label">label of object to return</param>
        public static List<XenRef<Blob>> get_by_name_label(Session session, string _label)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.blob_get_by_name_label(session.opaque_ref, _label);
            else
                return XenRef<Blob>.Create(session.proxy.blob_get_by_name_label(session.opaque_ref, _label ?? "").parse());
        }

        /// <summary>
        /// Get the uuid field of the given blob.
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_blob">The opaque_ref of the given blob</param>
        public static string get_uuid(Session session, string _blob)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.blob_get_uuid(session.opaque_ref, _blob);
            else
                return session.proxy.blob_get_uuid(session.opaque_ref, _blob ?? "").parse();
        }

        /// <summary>
        /// Get the name/label field of the given blob.
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_blob">The opaque_ref of the given blob</param>
        public static string get_name_label(Session session, string _blob)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.blob_get_name_label(session.opaque_ref, _blob);
            else
                return session.proxy.blob_get_name_label(session.opaque_ref, _blob ?? "").parse();
        }

        /// <summary>
        /// Get the name/description field of the given blob.
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_blob">The opaque_ref of the given blob</param>
        public static string get_name_description(Session session, string _blob)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.blob_get_name_description(session.opaque_ref, _blob);
            else
                return session.proxy.blob_get_name_description(session.opaque_ref, _blob ?? "").parse();
        }

        /// <summary>
        /// Get the size field of the given blob.
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_blob">The opaque_ref of the given blob</param>
        public static long get_size(Session session, string _blob)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.blob_get_size(session.opaque_ref, _blob);
            else
                return long.Parse(session.proxy.blob_get_size(session.opaque_ref, _blob ?? "").parse());
        }

        /// <summary>
        /// Get the public field of the given blob.
        /// First published in XenServer 6.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_blob">The opaque_ref of the given blob</param>
        public static bool get_public(Session session, string _blob)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.blob_get_public(session.opaque_ref, _blob);
            else
                return (bool)session.proxy.blob_get_public(session.opaque_ref, _blob ?? "").parse();
        }

        /// <summary>
        /// Get the last_updated field of the given blob.
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_blob">The opaque_ref of the given blob</param>
        public static DateTime get_last_updated(Session session, string _blob)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.blob_get_last_updated(session.opaque_ref, _blob);
            else
                return session.proxy.blob_get_last_updated(session.opaque_ref, _blob ?? "").parse();
        }

        /// <summary>
        /// Get the mime_type field of the given blob.
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_blob">The opaque_ref of the given blob</param>
        public static string get_mime_type(Session session, string _blob)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.blob_get_mime_type(session.opaque_ref, _blob);
            else
                return session.proxy.blob_get_mime_type(session.opaque_ref, _blob ?? "").parse();
        }

        /// <summary>
        /// Set the name/label field of the given blob.
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_blob">The opaque_ref of the given blob</param>
        /// <param name="_label">New value to set</param>
        public static void set_name_label(Session session, string _blob, string _label)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.blob_set_name_label(session.opaque_ref, _blob, _label);
            else
                session.proxy.blob_set_name_label(session.opaque_ref, _blob ?? "", _label ?? "").parse();
        }

        /// <summary>
        /// Set the name/description field of the given blob.
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_blob">The opaque_ref of the given blob</param>
        /// <param name="_description">New value to set</param>
        public static void set_name_description(Session session, string _blob, string _description)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.blob_set_name_description(session.opaque_ref, _blob, _description);
            else
                session.proxy.blob_set_name_description(session.opaque_ref, _blob ?? "", _description ?? "").parse();
        }

        /// <summary>
        /// Set the public field of the given blob.
        /// First published in XenServer 6.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_blob">The opaque_ref of the given blob</param>
        /// <param name="_public">New value to set</param>
        public static void set_public(Session session, string _blob, bool _public)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.blob_set_public(session.opaque_ref, _blob, _public);
            else
                session.proxy.blob_set_public(session.opaque_ref, _blob ?? "", _public).parse();
        }

        /// <summary>
        /// Create a placeholder for a binary blob
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_mime_type">The mime-type of the blob. Defaults to 'application/octet-stream' if the empty string is supplied</param>
        public static XenRef<Blob> create(Session session, string _mime_type)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.blob_create(session.opaque_ref, _mime_type);
            else
                return XenRef<Blob>.Create(session.proxy.blob_create(session.opaque_ref, _mime_type ?? "").parse());
        }

        /// <summary>
        /// Create a placeholder for a binary blob
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_mime_type">The mime-type of the blob. Defaults to 'application/octet-stream' if the empty string is supplied</param>
        /// <param name="_public">True if the blob should be publicly available First published in XenServer 6.1.</param>
        public static XenRef<Blob> create(Session session, string _mime_type, bool _public)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.blob_create(session.opaque_ref, _mime_type, _public);
            else
                return XenRef<Blob>.Create(session.proxy.blob_create(session.opaque_ref, _mime_type ?? "", _public).parse());
        }

        /// <summary>
        /// 
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_blob">The opaque_ref of the given blob</param>
        public static void destroy(Session session, string _blob)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.blob_destroy(session.opaque_ref, _blob);
            else
                session.proxy.blob_destroy(session.opaque_ref, _blob ?? "").parse();
        }

        /// <summary>
        /// Return a list of all the blobs known to the system.
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        public static List<XenRef<Blob>> get_all(Session session)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.blob_get_all(session.opaque_ref);
            else
                return XenRef<Blob>.Create(session.proxy.blob_get_all(session.opaque_ref).parse());
        }

        /// <summary>
        /// Get all the blob Records at once, in a single XML RPC call
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        public static Dictionary<XenRef<Blob>, Blob> get_all_records(Session session)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.blob_get_all_records(session.opaque_ref);
            else
                return XenRef<Blob>.Create<Proxy_Blob>(session.proxy.blob_get_all_records(session.opaque_ref).parse());
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
                    Changed = true;
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
                    Changed = true;
                    NotifyPropertyChanged("name_description");
                }
            }
        }
        private string _name_description = "";

        /// <summary>
        /// Size of the binary data, in bytes
        /// </summary>
        public virtual long size
        {
            get { return _size; }
            set
            {
                if (!Helper.AreEqual(value, _size))
                {
                    _size = value;
                    Changed = true;
                    NotifyPropertyChanged("size");
                }
            }
        }
        private long _size;

        /// <summary>
        /// True if the blob is publicly accessible
        /// First published in XenServer 6.1.
        /// </summary>
        public virtual bool pubblic
        {
            get { return _pubblic; }
            set
            {
                if (!Helper.AreEqual(value, _pubblic))
                {
                    _pubblic = value;
                    Changed = true;
                    NotifyPropertyChanged("pubblic");
                }
            }
        }
        private bool _pubblic = false;

        /// <summary>
        /// Time at which the data in the blob was last updated
        /// </summary>
        [JsonConverter(typeof(XenDateTimeConverter))]
        public virtual DateTime last_updated
        {
            get { return _last_updated; }
            set
            {
                if (!Helper.AreEqual(value, _last_updated))
                {
                    _last_updated = value;
                    Changed = true;
                    NotifyPropertyChanged("last_updated");
                }
            }
        }
        private DateTime _last_updated;

        /// <summary>
        /// The mime type associated with this object. Defaults to 'application/octet-stream' if the empty string is supplied
        /// </summary>
        public virtual string mime_type
        {
            get { return _mime_type; }
            set
            {
                if (!Helper.AreEqual(value, _mime_type))
                {
                    _mime_type = value;
                    Changed = true;
                    NotifyPropertyChanged("mime_type");
                }
            }
        }
        private string _mime_type = "";
    }
}
