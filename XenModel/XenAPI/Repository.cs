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
    /// Repository for updates
    /// First published in 1.301.0.
    /// </summary>
    public partial class Repository : XenObject<Repository>
    {
        #region Constructors

        public Repository()
        {
        }

        public Repository(string uuid,
            string name_label,
            string name_description,
            string binary_url,
            string source_url,
            bool update,
            string hash,
            bool up_to_date,
            string gpgkey_path,
            origin origin,
            string certificate)
        {
            this.uuid = uuid;
            this.name_label = name_label;
            this.name_description = name_description;
            this.binary_url = binary_url;
            this.source_url = source_url;
            this.update = update;
            this.hash = hash;
            this.up_to_date = up_to_date;
            this.gpgkey_path = gpgkey_path;
            this.origin = origin;
            this.certificate = certificate;
        }

        /// <summary>
        /// Creates a new Repository from a Hashtable.
        /// Note that the fields not contained in the Hashtable
        /// will be created with their default values.
        /// </summary>
        /// <param name="table"></param>
        public Repository(Hashtable table)
            : this()
        {
            UpdateFrom(table);
        }

        #endregion

        /// <summary>
        /// Updates each field of this instance with the value of
        /// the corresponding field of a given Repository.
        /// </summary>
        public override void UpdateFrom(Repository record)
        {
            uuid = record.uuid;
            name_label = record.name_label;
            name_description = record.name_description;
            binary_url = record.binary_url;
            source_url = record.source_url;
            update = record.update;
            hash = record.hash;
            up_to_date = record.up_to_date;
            gpgkey_path = record.gpgkey_path;
            origin = record.origin;
            certificate = record.certificate;
        }

        /// <summary>
        /// Given a Hashtable with field-value pairs, it updates the fields of this Repository
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
            if (table.ContainsKey("binary_url"))
                binary_url = Marshalling.ParseString(table, "binary_url");
            if (table.ContainsKey("source_url"))
                source_url = Marshalling.ParseString(table, "source_url");
            if (table.ContainsKey("update"))
                update = Marshalling.ParseBool(table, "update");
            if (table.ContainsKey("hash"))
                hash = Marshalling.ParseString(table, "hash");
            if (table.ContainsKey("up_to_date"))
                up_to_date = Marshalling.ParseBool(table, "up_to_date");
            if (table.ContainsKey("gpgkey_path"))
                gpgkey_path = Marshalling.ParseString(table, "gpgkey_path");
            if (table.ContainsKey("origin"))
                origin = (origin)Helper.EnumParseDefault(typeof(origin), Marshalling.ParseString(table, "origin"));
            if (table.ContainsKey("certificate"))
                certificate = Marshalling.ParseString(table, "certificate");
        }

        public bool DeepEquals(Repository other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;

            return Helper.AreEqual2(_uuid, other._uuid) &&
                Helper.AreEqual2(_name_label, other._name_label) &&
                Helper.AreEqual2(_name_description, other._name_description) &&
                Helper.AreEqual2(_binary_url, other._binary_url) &&
                Helper.AreEqual2(_source_url, other._source_url) &&
                Helper.AreEqual2(_update, other._update) &&
                Helper.AreEqual2(_hash, other._hash) &&
                Helper.AreEqual2(_up_to_date, other._up_to_date) &&
                Helper.AreEqual2(_gpgkey_path, other._gpgkey_path) &&
                Helper.AreEqual2(_origin, other._origin) &&
                Helper.AreEqual2(_certificate, other._certificate);
        }


        /// <summary>
        /// Get a record containing the current state of the given Repository.
        /// First published in 1.301.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_repository">The opaque_ref of the given repository</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static Repository get_record(Session session, string _repository)
        {
            return session.JsonRpcClient.repository_get_record(session.opaque_ref, _repository);
        }

        /// <summary>
        /// Get a reference to the Repository instance with the specified UUID.
        /// First published in 1.301.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_uuid">UUID of object to return</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static XenRef<Repository> get_by_uuid(Session session, string _uuid)
        {
            return session.JsonRpcClient.repository_get_by_uuid(session.opaque_ref, _uuid);
        }

        /// <summary>
        /// Get all the Repository instances with the given label.
        /// First published in 1.301.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_label">label of object to return</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static List<XenRef<Repository>> get_by_name_label(Session session, string _label)
        {
            return session.JsonRpcClient.repository_get_by_name_label(session.opaque_ref, _label);
        }

        /// <summary>
        /// Get the uuid field of the given Repository.
        /// First published in 1.301.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_repository">The opaque_ref of the given repository</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_uuid(Session session, string _repository)
        {
            return session.JsonRpcClient.repository_get_uuid(session.opaque_ref, _repository);
        }

        /// <summary>
        /// Get the name/label field of the given Repository.
        /// First published in 1.301.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_repository">The opaque_ref of the given repository</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_name_label(Session session, string _repository)
        {
            return session.JsonRpcClient.repository_get_name_label(session.opaque_ref, _repository);
        }

        /// <summary>
        /// Get the name/description field of the given Repository.
        /// First published in 1.301.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_repository">The opaque_ref of the given repository</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_name_description(Session session, string _repository)
        {
            return session.JsonRpcClient.repository_get_name_description(session.opaque_ref, _repository);
        }

        /// <summary>
        /// Get the binary_url field of the given Repository.
        /// First published in 1.301.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_repository">The opaque_ref of the given repository</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_binary_url(Session session, string _repository)
        {
            return session.JsonRpcClient.repository_get_binary_url(session.opaque_ref, _repository);
        }

        /// <summary>
        /// Get the source_url field of the given Repository.
        /// First published in 1.301.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_repository">The opaque_ref of the given repository</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_source_url(Session session, string _repository)
        {
            return session.JsonRpcClient.repository_get_source_url(session.opaque_ref, _repository);
        }

        /// <summary>
        /// Get the update field of the given Repository.
        /// First published in 1.301.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_repository">The opaque_ref of the given repository</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static bool get_update(Session session, string _repository)
        {
            return session.JsonRpcClient.repository_get_update(session.opaque_ref, _repository);
        }

        /// <summary>
        /// Get the hash field of the given Repository.
        /// First published in 1.301.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_repository">The opaque_ref of the given repository</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_hash(Session session, string _repository)
        {
            return session.JsonRpcClient.repository_get_hash(session.opaque_ref, _repository);
        }

        /// <summary>
        /// Get the up_to_date field of the given Repository.
        /// First published in 1.301.0.
        /// Deprecated since 23.18.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_repository">The opaque_ref of the given repository</param>
        [Deprecated("23.18.0")]
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static bool get_up_to_date(Session session, string _repository)
        {
            return session.JsonRpcClient.repository_get_up_to_date(session.opaque_ref, _repository);
        }

        /// <summary>
        /// Get the gpgkey_path field of the given Repository.
        /// Experimental. First published in 22.12.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_repository">The opaque_ref of the given repository</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_gpgkey_path(Session session, string _repository)
        {
            return session.JsonRpcClient.repository_get_gpgkey_path(session.opaque_ref, _repository);
        }

        /// <summary>
        /// Get the origin field of the given Repository.
        /// Experimental. First published in 24.23.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_repository">The opaque_ref of the given repository</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static origin get_origin(Session session, string _repository)
        {
            return session.JsonRpcClient.repository_get_origin(session.opaque_ref, _repository);
        }

        /// <summary>
        /// Get the certificate field of the given Repository.
        /// Experimental. First published in 25.7.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_repository">The opaque_ref of the given repository</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_certificate(Session session, string _repository)
        {
            return session.JsonRpcClient.repository_get_certificate(session.opaque_ref, _repository);
        }

        /// <summary>
        /// Set the name/label field of the given Repository.
        /// First published in 1.301.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_repository">The opaque_ref of the given repository</param>
        /// <param name="_label">New value to set</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void set_name_label(Session session, string _repository, string _label)
        {
            session.JsonRpcClient.repository_set_name_label(session.opaque_ref, _repository, _label);
        }

        /// <summary>
        /// Set the name/description field of the given Repository.
        /// First published in 1.301.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_repository">The opaque_ref of the given repository</param>
        /// <param name="_description">New value to set</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void set_name_description(Session session, string _repository, string _description)
        {
            session.JsonRpcClient.repository_set_name_description(session.opaque_ref, _repository, _description);
        }

        /// <summary>
        /// Add the configuration for a new remote repository
        /// First published in 1.301.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_name_label">The name of the repository</param>
        /// <param name="_name_description">The description of the repository</param>
        /// <param name="_binary_url">Base URL of binary packages in this repository</param>
        /// <param name="_source_url">Base URL of source packages in this repository</param>
        /// <param name="_update">True if the repository is an update repository. This means that updateinfo.xml will be parsed</param>
        /// <param name="_gpgkey_path">The GPG public key file name</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Repository> introduce(Session session, string _name_label, string _name_description, string _binary_url, string _source_url, bool _update, string _gpgkey_path)
        {
            return session.JsonRpcClient.repository_introduce(session.opaque_ref, _name_label, _name_description, _binary_url, _source_url, _update, _gpgkey_path);
        }

        /// <summary>
        /// Add the configuration for a new remote repository
        /// First published in 1.301.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_name_label">The name of the repository</param>
        /// <param name="_name_description">The description of the repository</param>
        /// <param name="_binary_url">Base URL of binary packages in this repository</param>
        /// <param name="_source_url">Base URL of source packages in this repository</param>
        /// <param name="_update">True if the repository is an update repository. This means that updateinfo.xml will be parsed</param>
        /// <param name="_gpgkey_path">The GPG public key file name</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Task> async_introduce(Session session, string _name_label, string _name_description, string _binary_url, string _source_url, bool _update, string _gpgkey_path)
        {
          return session.JsonRpcClient.async_repository_introduce(session.opaque_ref, _name_label, _name_description, _binary_url, _source_url, _update, _gpgkey_path);
        }

        /// <summary>
        /// Add the configuration for a new bundle repository
        /// Experimental. First published in 24.23.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_name_label">The name of the repository</param>
        /// <param name="_name_description">The description of the repository</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Repository> introduce_bundle(Session session, string _name_label, string _name_description)
        {
            return session.JsonRpcClient.repository_introduce_bundle(session.opaque_ref, _name_label, _name_description);
        }

        /// <summary>
        /// Add the configuration for a new bundle repository
        /// Experimental. First published in 24.23.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_name_label">The name of the repository</param>
        /// <param name="_name_description">The description of the repository</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Task> async_introduce_bundle(Session session, string _name_label, string _name_description)
        {
          return session.JsonRpcClient.async_repository_introduce_bundle(session.opaque_ref, _name_label, _name_description);
        }

        /// <summary>
        /// Add the configuration for a new remote pool repository
        /// Experimental. First published in 25.7.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_name_label">The name of the repository</param>
        /// <param name="_name_description">The description of the repository</param>
        /// <param name="_binary_url">Base URL of binary packages in the local repository of this remote pool in https://&lt;coordinator-ip&gt;/repository/enabled format</param>
        /// <param name="_certificate">The host certificate of the coordinator of the remote pool</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Repository> introduce_remote_pool(Session session, string _name_label, string _name_description, string _binary_url, string _certificate)
        {
            return session.JsonRpcClient.repository_introduce_remote_pool(session.opaque_ref, _name_label, _name_description, _binary_url, _certificate);
        }

        /// <summary>
        /// Add the configuration for a new remote pool repository
        /// Experimental. First published in 25.7.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_name_label">The name of the repository</param>
        /// <param name="_name_description">The description of the repository</param>
        /// <param name="_binary_url">Base URL of binary packages in the local repository of this remote pool in https://&lt;coordinator-ip&gt;/repository/enabled format</param>
        /// <param name="_certificate">The host certificate of the coordinator of the remote pool</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Task> async_introduce_remote_pool(Session session, string _name_label, string _name_description, string _binary_url, string _certificate)
        {
          return session.JsonRpcClient.async_repository_introduce_remote_pool(session.opaque_ref, _name_label, _name_description, _binary_url, _certificate);
        }

        /// <summary>
        /// Remove the repository record from the database
        /// First published in 1.301.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_repository">The opaque_ref of the given repository</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void forget(Session session, string _repository)
        {
            session.JsonRpcClient.repository_forget(session.opaque_ref, _repository);
        }

        /// <summary>
        /// Remove the repository record from the database
        /// First published in 1.301.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_repository">The opaque_ref of the given repository</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Task> async_forget(Session session, string _repository)
        {
          return session.JsonRpcClient.async_repository_forget(session.opaque_ref, _repository);
        }

        /// <summary>
        /// Set the file name of the GPG public key of the repository
        /// Experimental. First published in 22.12.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_repository">The opaque_ref of the given repository</param>
        /// <param name="_value">The file name of the GPG public key of the repository</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void set_gpgkey_path(Session session, string _repository, string _value)
        {
            session.JsonRpcClient.repository_set_gpgkey_path(session.opaque_ref, _repository, _value);
        }

        /// <summary>
        /// Set the file name of the GPG public key of the repository
        /// Experimental. First published in 22.12.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_repository">The opaque_ref of the given repository</param>
        /// <param name="_value">The file name of the GPG public key of the repository</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Task> async_set_gpgkey_path(Session session, string _repository, string _value)
        {
          return session.JsonRpcClient.async_repository_set_gpgkey_path(session.opaque_ref, _repository, _value);
        }

        /// <summary>
        /// Return a list of all the Repositorys known to the system.
        /// First published in 1.301.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static List<XenRef<Repository>> get_all(Session session)
        {
            return session.JsonRpcClient.repository_get_all(session.opaque_ref);
        }

        /// <summary>
        /// Return a map of Repository references to Repository records for all Repositorys known to the system.
        /// First published in 1.301.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static Dictionary<XenRef<Repository>, Repository> get_all_records(Session session)
        {
            return session.JsonRpcClient.repository_get_all_records(session.opaque_ref);
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
        /// Base URL of binary packages in this repository
        /// </summary>
        public virtual string binary_url
        {
            get { return _binary_url; }
            set
            {
                if (!Helper.AreEqual(value, _binary_url))
                {
                    _binary_url = value;
                    NotifyPropertyChanged("binary_url");
                }
            }
        }
        private string _binary_url = "";

        /// <summary>
        /// Base URL of source packages in this repository
        /// </summary>
        public virtual string source_url
        {
            get { return _source_url; }
            set
            {
                if (!Helper.AreEqual(value, _source_url))
                {
                    _source_url = value;
                    NotifyPropertyChanged("source_url");
                }
            }
        }
        private string _source_url = "";

        /// <summary>
        /// True if updateinfo.xml in this repository needs to be parsed
        /// </summary>
        public virtual bool update
        {
            get { return _update; }
            set
            {
                if (!Helper.AreEqual(value, _update))
                {
                    _update = value;
                    NotifyPropertyChanged("update");
                }
            }
        }
        private bool _update = false;

        /// <summary>
        /// SHA256 checksum of latest updateinfo.xml.gz in this repository if its 'update' is true
        /// </summary>
        public virtual string hash
        {
            get { return _hash; }
            set
            {
                if (!Helper.AreEqual(value, _hash))
                {
                    _hash = value;
                    NotifyPropertyChanged("hash");
                }
            }
        }
        private string _hash = "";

        /// <summary>
        /// True if all hosts in pool is up to date with this repository
        /// </summary>
        public virtual bool up_to_date
        {
            get { return _up_to_date; }
            set
            {
                if (!Helper.AreEqual(value, _up_to_date))
                {
                    _up_to_date = value;
                    NotifyPropertyChanged("up_to_date");
                }
            }
        }
        private bool _up_to_date = false;

        /// <summary>
        /// The file name of the GPG public key of this repository
        /// Experimental. First published in 22.12.0.
        /// </summary>
        public virtual string gpgkey_path
        {
            get { return _gpgkey_path; }
            set
            {
                if (!Helper.AreEqual(value, _gpgkey_path))
                {
                    _gpgkey_path = value;
                    NotifyPropertyChanged("gpgkey_path");
                }
            }
        }
        private string _gpgkey_path = "";

        /// <summary>
        /// The origin of this repository. 'remote' if the origin of the repository is a remote one, 'bundle' if the origin of the repository is a local bundle file, 'remote_pool' if the origin of the repository is a remote pool
        /// Experimental. First published in 24.23.0.
        /// </summary>
        [JsonConverter(typeof(originConverter))]
        public virtual origin origin
        {
            get { return _origin; }
            set
            {
                if (!Helper.AreEqual(value, _origin))
                {
                    _origin = value;
                    NotifyPropertyChanged("origin");
                }
            }
        }
        private origin _origin = origin.remote;

        /// <summary>
        /// The certificate of the host which hosts this repository
        /// Experimental. First published in 25.7.0.
        /// </summary>
        public virtual string certificate
        {
            get { return _certificate; }
            set
            {
                if (!Helper.AreEqual(value, _certificate))
                {
                    _certificate = value;
                    NotifyPropertyChanged("certificate");
                }
            }
        }
        private string _certificate = "";
    }
}
