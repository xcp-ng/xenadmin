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
    /// A set of high-level properties associated with an SR.
    /// First published in XenServer 7.6.
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

        #endregion

        /// <summary>
        /// Updates each field of this instance with the value of
        /// the corresponding field of a given Sr_stat.
        /// </summary>
        public override void UpdateFrom(Sr_stat record)
        {
            uuid = record.uuid;
            name_label = record.name_label;
            name_description = record.name_description;
            free_space = record.free_space;
            total_space = record.total_space;
            clustered = record.clustered;
            health = record.health;
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

            return Helper.AreEqual2(_uuid, other._uuid) &&
                Helper.AreEqual2(_name_label, other._name_label) &&
                Helper.AreEqual2(_name_description, other._name_description) &&
                Helper.AreEqual2(_free_space, other._free_space) &&
                Helper.AreEqual2(_total_space, other._total_space) &&
                Helper.AreEqual2(_clustered, other._clustered) &&
                Helper.AreEqual2(_health, other._health);
        }


        /// <summary>
        /// Uuid that uniquely identifies this SR, if one is available.
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
        private string _uuid;

        /// <summary>
        /// Short, human-readable label for the SR.
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
        /// Longer, human-readable description of the SR. Descriptions are generally only displayed by clients when the user is examining SRs in detail.
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
        /// Number of bytes free on the backing storage (in bytes)
        /// </summary>
        public virtual long free_space
        {
            get { return _free_space; }
            set
            {
                if (!Helper.AreEqual(value, _free_space))
                {
                    _free_space = value;
                    NotifyPropertyChanged("free_space");
                }
            }
        }
        private long _free_space;

        /// <summary>
        /// Total physical size of the backing storage (in bytes)
        /// </summary>
        public virtual long total_space
        {
            get { return _total_space; }
            set
            {
                if (!Helper.AreEqual(value, _total_space))
                {
                    _total_space = value;
                    NotifyPropertyChanged("total_space");
                }
            }
        }
        private long _total_space;

        /// <summary>
        /// Indicates whether the SR uses clustered local storage.
        /// </summary>
        public virtual bool clustered
        {
            get { return _clustered; }
            set
            {
                if (!Helper.AreEqual(value, _clustered))
                {
                    _clustered = value;
                    NotifyPropertyChanged("clustered");
                }
            }
        }
        private bool _clustered;

        /// <summary>
        /// The health status of the SR.
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
                    NotifyPropertyChanged("health");
                }
            }
        }
        private sr_health _health;
    }
}
