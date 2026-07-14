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
    /// LVHD SR specific operations
    /// First published in XenServer 7.0.
    /// </summary>
    public partial class LVHD : XenObject<LVHD>
    {
        #region Constructors

        public LVHD()
        {
        }

        public LVHD(string uuid)
        {
            this.uuid = uuid;
        }

        /// <summary>
        /// Creates a new LVHD from a Hashtable.
        /// Note that the fields not contained in the Hashtable
        /// will be created with their default values.
        /// </summary>
        /// <param name="table"></param>
        public LVHD(Hashtable table)
            : this()
        {
            UpdateFrom(table);
        }

        #endregion

        /// <summary>
        /// Updates each field of this instance with the value of
        /// the corresponding field of a given LVHD.
        /// </summary>
        public override void UpdateFrom(LVHD record)
        {
            uuid = record.uuid;
        }

        /// <summary>
        /// Given a Hashtable with field-value pairs, it updates the fields of this LVHD
        /// with the values listed in the Hashtable. Note that only the fields contained
        /// in the Hashtable will be updated and the rest will remain the same.
        /// </summary>
        /// <param name="table"></param>
        public void UpdateFrom(Hashtable table)
        {
            if (table.ContainsKey("uuid"))
                uuid = Marshalling.ParseString(table, "uuid");
        }

        public bool DeepEquals(LVHD other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;

            return Helper.AreEqual2(_uuid, other._uuid);
        }


        /// <summary>
        /// Get a record containing the current state of the given LVHD.
        /// First published in XenServer 7.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_lvhd">The opaque_ref of the given lvhd</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static LVHD get_record(Session session, string _lvhd)
        {
            return session.JsonRpcClient.lvhd_get_record(session.opaque_ref, _lvhd);
        }

        /// <summary>
        /// Get a reference to the LVHD instance with the specified UUID.
        /// First published in XenServer 7.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_uuid">UUID of object to return</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static XenRef<LVHD> get_by_uuid(Session session, string _uuid)
        {
            return session.JsonRpcClient.lvhd_get_by_uuid(session.opaque_ref, _uuid);
        }

        /// <summary>
        /// Get the uuid field of the given LVHD.
        /// First published in XenServer 7.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_lvhd">The opaque_ref of the given lvhd</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_uuid(Session session, string _lvhd)
        {
            return session.JsonRpcClient.lvhd_get_uuid(session.opaque_ref, _lvhd);
        }

        /// <summary>
        /// Upgrades an LVHD SR to enable thin-provisioning. Future VDIs created in this SR will be thinly-provisioned, although existing VDIs will be left alone. Note that the SR must be attached to the SRmaster for upgrade to work.
        /// First published in XenServer 7.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The LVHD Host to upgrade to being thin-provisioned.</param>
        /// <param name="_sr">The LVHD SR to upgrade to being thin-provisioned.</param>
        /// <param name="_initial_allocation">The initial amount of space to allocate to a newly-created VDI in bytes</param>
        /// <param name="_allocation_quantum">The amount of space to allocate to a VDI when it needs to be enlarged in bytes</param>
        /// <remarks>
        /// Minimum allowed role: pool-admin
        /// </remarks>
        public static string enable_thin_provisioning(Session session, string _host, string _sr, long _initial_allocation, long _allocation_quantum)
        {
            return session.JsonRpcClient.lvhd_enable_thin_provisioning(session.opaque_ref, _host, _sr, _initial_allocation, _allocation_quantum);
        }

        /// <summary>
        /// Upgrades an LVHD SR to enable thin-provisioning. Future VDIs created in this SR will be thinly-provisioned, although existing VDIs will be left alone. Note that the SR must be attached to the SRmaster for upgrade to work.
        /// First published in XenServer 7.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The LVHD Host to upgrade to being thin-provisioned.</param>
        /// <param name="_sr">The LVHD SR to upgrade to being thin-provisioned.</param>
        /// <param name="_initial_allocation">The initial amount of space to allocate to a newly-created VDI in bytes</param>
        /// <param name="_allocation_quantum">The amount of space to allocate to a VDI when it needs to be enlarged in bytes</param>
        /// <remarks>
        /// Minimum allowed role: pool-admin
        /// </remarks>
        public static XenRef<Task> async_enable_thin_provisioning(Session session, string _host, string _sr, long _initial_allocation, long _allocation_quantum)
        {
          return session.JsonRpcClient.async_lvhd_enable_thin_provisioning(session.opaque_ref, _host, _sr, _initial_allocation, _allocation_quantum);
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
    }
}
