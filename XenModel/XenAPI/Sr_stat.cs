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
    /// A set of high-level properties associated with an SR.
    /// </summary>
    public partial class Sr_stat : XenObject<Sr_stat>
    {
        #region Constructors

        public Sr_stat()
        {
        }

        public Sr_stat(string uuid,
            string name_label,
            string name_description,
            long free_space,
            long total_space,
            bool clustered,
            sr_health health)
        {
            this.uuid = uuid;
            this.name_label = name_label;
            this.name_description = name_description;
            this.free_space = free_space;
            this.total_space = total_space;
            this.clustered = clustered;
            this.health = health;
        }

        /// <summary>
        /// Creates a new Sr_stat from a Hashtable.
        /// Note that the fields not contained in the Hashtable
        /// will be created with their default values.
        /// </summary>
        /// <param name="table"></param>
        public Sr_stat(Hashtable table)
            : this()
        {
            UpdateFrom(table);
        }

        /// <summary>
        /// Creates a new Sr_stat from a Proxy_Sr_stat.
        /// </summary>
        /// <param name="proxy"></param>
        public Sr_stat(Proxy_Sr_stat proxy)
        {
            UpdateFrom(proxy);
        }

        #endregion

        /// <summary>
        /// Updates each field of this instance with the value of
        /// the corresponding field of a given Sr_stat.
        /// </summary>
        public override void UpdateFrom(Sr_stat update)
        {
            uuid = update.uuid;
            name_label = update.name_label;
            name_description = update.name_description;
            free_space = update.free_space;
            total_space = update.total_space;
            clustered = update.clustered;
            health = update.health;
        }

        internal void UpdateFrom(Proxy_Sr_stat proxy)
        {
            uuid = proxy.uuid == null ? null : proxy.uuid;
            name_label = proxy.name_label == null ? null : proxy.name_label;
            name_description = proxy.name_description == null ? null : proxy.name_description;
            free_space = proxy.free_space == null ? 0 : long.Parse(proxy.free_space);
            total_space = proxy.total_space == null ? 0 : long.Parse(proxy.total_space);
            clustered = (bool)proxy.clustered;
            health = proxy.health == null ? (sr_health) 0 : (sr_health)Helper.EnumParseDefault(typeof(sr_health), (string)proxy.health);
        }

        public Proxy_Sr_stat ToProxy()
        {
            Proxy_Sr_stat result_ = new Proxy_Sr_stat();
            result_.uuid = uuid;
            result_.name_label = name_label ?? "";
            result_.name_description = name_description ?? "";
            result_.free_space = free_space.ToString();
            result_.total_space = total_space.ToString();
            result_.clustered = clustered;
            result_.health = sr_health_helper.ToString(health);
            return result_;
        }

        /// <summary>
        /// Given a Hashtable with field-value pairs, it updates the fields of this Sr_stat
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
            if (table.ContainsKey("free_space"))
                free_space = Marshalling.ParseLong(table, "free_space");
            if (table.ContainsKey("total_space"))
                total_space = Marshalling.ParseLong(table, "total_space");
            if (table.ContainsKey("clustered"))
                clustered = Marshalling.ParseBool(table, "clustered");
            if (table.ContainsKey("health"))
                health = (sr_health)Helper.EnumParseDefault(typeof(sr_health), Marshalling.ParseString(table, "health"));
        }

        public bool DeepEquals(Sr_stat other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;

            return Helper.AreEqual2(this._uuid, other._uuid) &&
                Helper.AreEqual2(this._name_label, other._name_label) &&
                Helper.AreEqual2(this._name_description, other._name_description) &&
                Helper.AreEqual2(this._free_space, other._free_space) &&
                Helper.AreEqual2(this._total_space, other._total_space) &&
                Helper.AreEqual2(this._clustered, other._clustered) &&
                Helper.AreEqual2(this._health, other._health);
        }

        internal static List<Sr_stat> ProxyArrayToObjectList(Proxy_Sr_stat[] input)
        {
            var result = new List<Sr_stat>();
            foreach (var item in input)
                result.Add(new Sr_stat(item));

            return result;
        }

        public override string SaveChanges(Session session, string opaqueRef, Sr_stat server)
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
        /// Uuid that uniquely identifies this SR, if one is available.
        /// Experimental. First published in XenServer 7.5.
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
        private string _uuid;

        /// <summary>
        /// Short, human-readable label for the SR.
        /// Experimental. First published in XenServer 7.5.
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
        /// Longer, human-readable description of the SR. Descriptions are generally only displayed by clients when the user is examining SRs in detail.
        /// Experimental. First published in XenServer 7.5.
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
        /// Number of bytes free on the backing storage (in bytes)
        /// Experimental. First published in XenServer 7.5.
        /// </summary>
        public virtual long free_space
        {
            get { return _free_space; }
            set
            {
                if (!Helper.AreEqual(value, _free_space))
                {
                    _free_space = value;
                    Changed = true;
                    NotifyPropertyChanged("free_space");
                }
            }
        }
        private long _free_space;

        /// <summary>
        /// Total physical size of the backing storage (in bytes)
        /// Experimental. First published in XenServer 7.5.
        /// </summary>
        public virtual long total_space
        {
            get { return _total_space; }
            set
            {
                if (!Helper.AreEqual(value, _total_space))
                {
                    _total_space = value;
                    Changed = true;
                    NotifyPropertyChanged("total_space");
                }
            }
        }
        private long _total_space;

        /// <summary>
        /// Indicates whether the SR uses clustered local storage.
        /// Experimental. First published in XenServer 7.5.
        /// </summary>
        public virtual bool clustered
        {
            get { return _clustered; }
            set
            {
                if (!Helper.AreEqual(value, _clustered))
                {
                    _clustered = value;
                    Changed = true;
                    NotifyPropertyChanged("clustered");
                }
            }
        }
        private bool _clustered;

        /// <summary>
        /// The health status of the SR.
        /// Experimental. First published in XenServer 7.5.
        /// </summary>
        [JsonConverter(typeof(sr_healthConverter))]
        public virtual sr_health health
        {
            get { return _health; }
            set
            {
                if (!Helper.AreEqual(value, _health))
                {
                    _health = value;
                    Changed = true;
                    NotifyPropertyChanged("health");
                }
            }
        }
        private sr_health _health;
    }
}
