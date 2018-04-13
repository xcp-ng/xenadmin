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
    /// Details for connecting to a VDI using the Network Block Device protocol
    /// First published in XenServer 7.3.
    /// </summary>
    public partial class Vdi_nbd_server_info : XenObject<Vdi_nbd_server_info>
    {
        public Vdi_nbd_server_info()
        {
        }

        public Vdi_nbd_server_info(string exportname,
            string address,
            long port,
            string cert,
            string subject)
        {
            this.exportname = exportname;
            this.address = address;
            this.port = port;
            this.cert = cert;
            this.subject = subject;
        }

        /// <summary>
        /// Creates a new Vdi_nbd_server_info from a Proxy_Vdi_nbd_server_info.
        /// </summary>
        /// <param name="proxy"></param>
        public Vdi_nbd_server_info(Proxy_Vdi_nbd_server_info proxy)
        {
            this.UpdateFromProxy(proxy);
        }

        /// <summary>
        /// Updates each field of this instance with the value of
        /// the corresponding field of a given Vdi_nbd_server_info.
        /// </summary>
        public override void UpdateFrom(Vdi_nbd_server_info update)
        {
            exportname = update.exportname;
            address = update.address;
            port = update.port;
            cert = update.cert;
            subject = update.subject;
        }

        internal void UpdateFromProxy(Proxy_Vdi_nbd_server_info proxy)
        {
            exportname = proxy.exportname == null ? null : proxy.exportname;
            address = proxy.address == null ? null : proxy.address;
            port = proxy.port == null ? 0 : long.Parse(proxy.port);
            cert = proxy.cert == null ? null : proxy.cert;
            subject = proxy.subject == null ? null : proxy.subject;
        }

        public Proxy_Vdi_nbd_server_info ToProxy()
        {
            Proxy_Vdi_nbd_server_info result_ = new Proxy_Vdi_nbd_server_info();
            result_.exportname = exportname ?? "";
            result_.address = address ?? "";
            result_.port = port.ToString();
            result_.cert = cert ?? "";
            result_.subject = subject ?? "";
            return result_;
        }

        /// <summary>
        /// Creates a new Vdi_nbd_server_info from a Hashtable.
        /// Note that the fields not contained in the Hashtable
        /// will be created with their default values.
        /// </summary>
        /// <param name="table"></param>
        public Vdi_nbd_server_info(Hashtable table) : this()
        {
            UpdateFrom(table);
        }

        /// <summary>
        /// Given a Hashtable with field-value pairs, it updates the fields of this Vdi_nbd_server_info
        /// with the values listed in the Hashtable. Note that only the fields contained
        /// in the Hashtable will be updated and the rest will remain the same.
        /// </summary>
        /// <param name="table"></param>
        public void UpdateFrom(Hashtable table)
        {
            if (table.ContainsKey("exportname"))
                exportname = Marshalling.ParseString(table, "exportname");
            if (table.ContainsKey("address"))
                address = Marshalling.ParseString(table, "address");
            if (table.ContainsKey("port"))
                port = Marshalling.ParseLong(table, "port");
            if (table.ContainsKey("cert"))
                cert = Marshalling.ParseString(table, "cert");
            if (table.ContainsKey("subject"))
                subject = Marshalling.ParseString(table, "subject");
        }

        public bool DeepEquals(Vdi_nbd_server_info other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;

            return Helper.AreEqual2(this._exportname, other._exportname) &&
                Helper.AreEqual2(this._address, other._address) &&
                Helper.AreEqual2(this._port, other._port) &&
                Helper.AreEqual2(this._cert, other._cert) &&
                Helper.AreEqual2(this._subject, other._subject);
        }

        internal static List<Vdi_nbd_server_info> ProxyArrayToObjectList(Proxy_Vdi_nbd_server_info[] input)
        {
            var result = new List<Vdi_nbd_server_info>();
            foreach (var item in input)
                result.Add(new Vdi_nbd_server_info(item));

            return result;
        }

        public override string SaveChanges(Session session, string opaqueRef, Vdi_nbd_server_info server)
        {
            if (opaqueRef == null)
            {
                System.Diagnostics.Debug.Assert(false, "Cannot create instances of this type on the server");
                return "";
            }
            else
            {
              throw new InvalidOperationException("This type has no read/write properties");
            }
        }
        /// <summary>
        /// The exportname to request over NBD. This holds details including an authentication token, so it must be protected appropriately. Clients should regard the exportname as an opaque string or token.
        /// </summary>
        public virtual string exportname
        {
            get { return _exportname; }
            set
            {
                if (!Helper.AreEqual(value, _exportname))
                {
                    _exportname = value;
                    Changed = true;
                    NotifyPropertyChanged("exportname");
                }
            }
        }
        private string _exportname = "";

        /// <summary>
        /// An address on which the server can be reached; this can be IPv4, IPv6, or a DNS name.
        /// </summary>
        public virtual string address
        {
            get { return _address; }
            set
            {
                if (!Helper.AreEqual(value, _address))
                {
                    _address = value;
                    Changed = true;
                    NotifyPropertyChanged("address");
                }
            }
        }
        private string _address = "";

        /// <summary>
        /// The TCP port
        /// </summary>
        public virtual long port
        {
            get { return _port; }
            set
            {
                if (!Helper.AreEqual(value, _port))
                {
                    _port = value;
                    Changed = true;
                    NotifyPropertyChanged("port");
                }
            }
        }
        private long _port;

        /// <summary>
        /// The TLS certificate of the server
        /// </summary>
        public virtual string cert
        {
            get { return _cert; }
            set
            {
                if (!Helper.AreEqual(value, _cert))
                {
                    _cert = value;
                    Changed = true;
                    NotifyPropertyChanged("cert");
                }
            }
        }
        private string _cert = "";

        /// <summary>
        /// For convenience, this redundant field holds a DNS (hostname) subject of the certificate. This can be a wildcard, but only for a certificate that has a wildcard subject and no concrete hostname subjects.
        /// </summary>
        public virtual string subject
        {
            get { return _subject; }
            set
            {
                if (!Helper.AreEqual(value, _subject))
                {
                    _subject = value;
                    Changed = true;
                    NotifyPropertyChanged("subject");
                }
            }
        }
        private string _subject = "";
    }
}
