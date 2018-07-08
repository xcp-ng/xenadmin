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
    /// A set of properties that describe one result element of SR.probe. Result elements and properties can change dynamically based on changes to the the SR.probe input-parameters or the target.
    /// </summary>
    public partial class Probe_result : XenObject<Probe_result>
    {
        public Probe_result()
        {
        }

        public Probe_result(Dictionary<string, string> configuration,
            bool complete,
            Sr_stat sr,
            Dictionary<string, string> extra_info)
        {
            this.configuration = configuration;
            this.complete = complete;
            this.sr = sr;
            this.extra_info = extra_info;
        }

        /// <summary>
        /// Creates a new Probe_result from a Proxy_Probe_result.
        /// </summary>
        /// <param name="proxy"></param>
        public Probe_result(Proxy_Probe_result proxy)
        {
            this.UpdateFromProxy(proxy);
        }

        /// <summary>
        /// Updates each field of this instance with the value of
        /// the corresponding field of a given Probe_result.
        /// </summary>
        public override void UpdateFrom(Probe_result update)
        {
            configuration = update.configuration;
            complete = update.complete;
            sr = update.sr;
            extra_info = update.extra_info;
        }

        internal void UpdateFromProxy(Proxy_Probe_result proxy)
        {
            configuration = proxy.configuration == null ? null : Maps.convert_from_proxy_string_string(proxy.configuration);
            complete = (bool)proxy.complete;
            sr = proxy.sr == null ? null : new Sr_stat(proxy.sr);
            extra_info = proxy.extra_info == null ? null : Maps.convert_from_proxy_string_string(proxy.extra_info);
        }

        public Proxy_Probe_result ToProxy()
        {
            Proxy_Probe_result result_ = new Proxy_Probe_result();
            result_.configuration = Maps.convert_to_proxy_string_string(configuration);
            result_.complete = complete;
            result_.sr = sr == null ? null : sr.ToProxy();
            result_.extra_info = Maps.convert_to_proxy_string_string(extra_info);
            return result_;
        }

        /// <summary>
        /// Creates a new Probe_result from a Hashtable.
        /// Note that the fields not contained in the Hashtable
        /// will be created with their default values.
        /// </summary>
        /// <param name="table"></param>
        public Probe_result(Hashtable table) : this()
        {
            UpdateFrom(table);
        }

        /// <summary>
        /// Given a Hashtable with field-value pairs, it updates the fields of this Probe_result
        /// with the values listed in the Hashtable. Note that only the fields contained
        /// in the Hashtable will be updated and the rest will remain the same.
        /// </summary>
        /// <param name="table"></param>
        public void UpdateFrom(Hashtable table)
        {
            if (table.ContainsKey("configuration"))
                configuration = Maps.convert_from_proxy_string_string(Marshalling.ParseHashTable(table, "configuration"));
            if (table.ContainsKey("complete"))
                complete = Marshalling.ParseBool(table, "complete");
            if (table.ContainsKey("sr"))
                sr = (Sr_stat)Marshalling.convertStruct(typeof(Sr_stat), Marshalling.ParseHashTable(table, "sr"));;
            if (table.ContainsKey("extra_info"))
                extra_info = Maps.convert_from_proxy_string_string(Marshalling.ParseHashTable(table, "extra_info"));
        }

        public bool DeepEquals(Probe_result other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;

            return Helper.AreEqual2(this._configuration, other._configuration) &&
                Helper.AreEqual2(this._complete, other._complete) &&
                Helper.AreEqual2(this._sr, other._sr) &&
                Helper.AreEqual2(this._extra_info, other._extra_info);
        }

        internal static List<Probe_result> ProxyArrayToObjectList(Proxy_Probe_result[] input)
        {
            var result = new List<Probe_result>();
            foreach (var item in input)
                result.Add(new Probe_result(item));

            return result;
        }

        public override string SaveChanges(Session session, string opaqueRef, Probe_result server)
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
        /// Plugin-specific configuration which describes where and how to locate the storage repository. This may include the physical block device name, a remote NFS server and path or an RBD storage pool.
        /// Experimental. First published in XenServer 7.5.
        /// </summary>
        [JsonConverter(typeof(StringStringMapConverter))]
        public virtual Dictionary<string, string> configuration
        {
            get { return _configuration; }
            set
            {
                if (!Helper.AreEqual(value, _configuration))
                {
                    _configuration = value;
                    Changed = true;
                    NotifyPropertyChanged("configuration");
                }
            }
        }
        private Dictionary<string, string> _configuration = new Dictionary<string, string>() {};

        /// <summary>
        /// True if this configuration is complete and can be used to call SR.create. False if it requires further iterative calls to SR.probe, to potentially narrow down on a configuration that can be used.
        /// Experimental. First published in XenServer 7.5.
        /// </summary>
        public virtual bool complete
        {
            get { return _complete; }
            set
            {
                if (!Helper.AreEqual(value, _complete))
                {
                    _complete = value;
                    Changed = true;
                    NotifyPropertyChanged("complete");
                }
            }
        }
        private bool _complete;

        /// <summary>
        /// Existing SR found for this configuration
        /// Experimental. First published in XenServer 7.5.
        /// </summary>
        public virtual Sr_stat sr
        {
            get { return _sr; }
            set
            {
                if (!Helper.AreEqual(value, _sr))
                {
                    _sr = value;
                    Changed = true;
                    NotifyPropertyChanged("sr");
                }
            }
        }
        private Sr_stat _sr;

        /// <summary>
        /// Additional plugin-specific information about this configuration, that might be of use for an API user. This can for example include the LUN or the WWPN.
        /// Experimental. First published in XenServer 7.5.
        /// </summary>
        [JsonConverter(typeof(StringStringMapConverter))]
        public virtual Dictionary<string, string> extra_info
        {
            get { return _extra_info; }
            set
            {
                if (!Helper.AreEqual(value, _extra_info))
                {
                    _extra_info = value;
                    Changed = true;
                    NotifyPropertyChanged("extra_info");
                }
            }
        }
        private Dictionary<string, string> _extra_info = new Dictionary<string, string>() {};
    }
}
