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
    /// DR task
    /// First published in XenServer 6.0.
    /// </summary>
    public partial class DR_task : XenObject<DR_task>
    {
        #region Constructors

        public DR_task()
        {
        }

        public DR_task(string uuid,
            List<XenRef<SR>> introduced_SRs)
        {
            this.uuid = uuid;
            this.introduced_SRs = introduced_SRs;
        }

        /// <summary>
        /// Creates a new DR_task from a Hashtable.
        /// Note that the fields not contained in the Hashtable
        /// will be created with their default values.
        /// </summary>
        /// <param name="table"></param>
        public DR_task(Hashtable table)
            : this()
        {
            UpdateFrom(table);
        }

        #endregion

        /// <summary>
        /// Updates each field of this instance with the value of
        /// the corresponding field of a given DR_task.
        /// </summary>
        public override void UpdateFrom(DR_task record)
        {
            uuid = record.uuid;
            introduced_SRs = record.introduced_SRs;
        }

        /// <summary>
        /// Given a Hashtable with field-value pairs, it updates the fields of this DR_task
        /// with the values listed in the Hashtable. Note that only the fields contained
        /// in the Hashtable will be updated and the rest will remain the same.
        /// </summary>
        /// <param name="table"></param>
        public void UpdateFrom(Hashtable table)
        {
            if (table.ContainsKey("uuid"))
                uuid = Marshalling.ParseString(table, "uuid");
            if (table.ContainsKey("introduced_SRs"))
                introduced_SRs = Marshalling.ParseSetRef<SR>(table, "introduced_SRs");
        }

        public bool DeepEquals(DR_task other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;

            return Helper.AreEqual2(_uuid, other._uuid) &&
                Helper.AreEqual2(_introduced_SRs, other._introduced_SRs);
        }


        /// <summary>
        /// Get a record containing the current state of the given DR_task.
        /// First published in XenServer 6.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_dr_task">The opaque_ref of the given dr_task</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static DR_task get_record(Session session, string _dr_task)
        {
            return session.JsonRpcClient.dr_task_get_record(session.opaque_ref, _dr_task);
        }

        /// <summary>
        /// Get a reference to the DR_task instance with the specified UUID.
        /// First published in XenServer 6.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_uuid">UUID of object to return</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static XenRef<DR_task> get_by_uuid(Session session, string _uuid)
        {
            return session.JsonRpcClient.dr_task_get_by_uuid(session.opaque_ref, _uuid);
        }

        /// <summary>
        /// Get the uuid field of the given DR_task.
        /// First published in XenServer 6.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_dr_task">The opaque_ref of the given dr_task</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_uuid(Session session, string _dr_task)
        {
            return session.JsonRpcClient.dr_task_get_uuid(session.opaque_ref, _dr_task);
        }

        /// <summary>
        /// Get the introduced_SRs field of the given DR_task.
        /// First published in XenServer 6.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_dr_task">The opaque_ref of the given dr_task</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static List<XenRef<SR>> get_introduced_SRs(Session session, string _dr_task)
        {
            return session.JsonRpcClient.dr_task_get_introduced_srs(session.opaque_ref, _dr_task);
        }

        /// <summary>
        /// Create a disaster recovery task which will query the supplied list of devices
        /// First published in XenServer 6.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_type">The SR driver type of the SRs to introduce</param>
        /// <param name="_device_config">The device configuration of the SRs to introduce</param>
        /// <param name="_whitelist">The devices to use for disaster recovery</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<DR_task> create(Session session, string _type, Dictionary<string, string> _device_config, string[] _whitelist)
        {
            return session.JsonRpcClient.dr_task_create(session.opaque_ref, _type, _device_config, _whitelist);
        }

        /// <summary>
        /// Create a disaster recovery task which will query the supplied list of devices
        /// First published in XenServer 6.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_type">The SR driver type of the SRs to introduce</param>
        /// <param name="_device_config">The device configuration of the SRs to introduce</param>
        /// <param name="_whitelist">The devices to use for disaster recovery</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Task> async_create(Session session, string _type, Dictionary<string, string> _device_config, string[] _whitelist)
        {
          return session.JsonRpcClient.async_dr_task_create(session.opaque_ref, _type, _device_config, _whitelist);
        }

        /// <summary>
        /// Destroy the disaster recovery task, detaching and forgetting any SRs introduced which are no longer required
        /// First published in XenServer 6.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_dr_task">The opaque_ref of the given dr_task</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static void destroy(Session session, string _dr_task)
        {
            session.JsonRpcClient.dr_task_destroy(session.opaque_ref, _dr_task);
        }

        /// <summary>
        /// Destroy the disaster recovery task, detaching and forgetting any SRs introduced which are no longer required
        /// First published in XenServer 6.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_dr_task">The opaque_ref of the given dr_task</param>
        /// <remarks>
        /// Minimum allowed role: pool-operator
        /// </remarks>
        public static XenRef<Task> async_destroy(Session session, string _dr_task)
        {
          return session.JsonRpcClient.async_dr_task_destroy(session.opaque_ref, _dr_task);
        }

        /// <summary>
        /// Return a list of all the DR_tasks known to the system.
        /// First published in XenServer 6.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static List<XenRef<DR_task>> get_all(Session session)
        {
            return session.JsonRpcClient.dr_task_get_all(session.opaque_ref);
        }

        /// <summary>
        /// Return a map of DR_task references to DR_task records for all DR_tasks known to the system.
        /// First published in XenServer 6.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static Dictionary<XenRef<DR_task>, DR_task> get_all_records(Session session)
        {
            return session.JsonRpcClient.dr_task_get_all_records(session.opaque_ref);
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
        /// All SRs introduced by this appliance
        /// </summary>
        [JsonConverter(typeof(XenRefListConverter<SR>))]
        public virtual List<XenRef<SR>> introduced_SRs
        {
            get { return _introduced_SRs; }
            set
            {
                if (!Helper.AreEqual(value, _introduced_SRs))
                {
                    _introduced_SRs = value;
                    NotifyPropertyChanged("introduced_SRs");
                }
            }
        }
        private List<XenRef<SR>> _introduced_SRs = new List<XenRef<SR>>() {};
    }
}
