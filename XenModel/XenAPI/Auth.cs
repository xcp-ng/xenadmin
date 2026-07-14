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
    /// Management of remote authentication services
    /// First published in XenServer 5.5.
    /// </summary>
    public partial class Auth : XenObject<Auth>
    {
        #region Constructors

        public Auth()
        {
        }

        /// <summary>
        /// Creates a new Auth from a Hashtable.
        /// Note that the fields not contained in the Hashtable
        /// will be created with their default values.
        /// </summary>
        /// <param name="table"></param>
        public Auth(Hashtable table)
            : this()
        {
            UpdateFrom(table);
        }

        #endregion

        /// <summary>
        /// Updates each field of this instance with the value of
        /// the corresponding field of a given Auth.
        /// </summary>
        public override void UpdateFrom(Auth record)
        {
        }

        /// <summary>
        /// Given a Hashtable with field-value pairs, it updates the fields of this Auth
        /// with the values listed in the Hashtable. Note that only the fields contained
        /// in the Hashtable will be updated and the rest will remain the same.
        /// </summary>
        /// <param name="table"></param>
        public void UpdateFrom(Hashtable table)
        {
        }

        public bool DeepEquals(Auth other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;

            return false;
        }


        /// <summary>
        /// This call queries the external directory service to obtain the subject_identifier as a string from the human-readable subject_name
        /// First published in XenServer 5.5.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_subject_name">The human-readable subject_name, such as a username or a groupname</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string get_subject_identifier(Session session, string _subject_name)
        {
            return session.JsonRpcClient.auth_get_subject_identifier(session.opaque_ref, _subject_name);
        }

        /// <summary>
        /// This call queries the external directory service to obtain the user information (for example, username, organization etc.) from the specified subject_identifier
        /// First published in XenServer 5.5.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_subject_identifier">A string containing the subject_identifier, unique in the external directory service</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static Dictionary<string, string> get_subject_information_from_identifier(Session session, string _subject_identifier)
        {
            return session.JsonRpcClient.auth_get_subject_information_from_identifier(session.opaque_ref, _subject_identifier);
        }

        /// <summary>
        /// This calls queries the external directory service to obtain the transitively-closed set of groups that the the subject_identifier is member of.
        /// First published in XenServer 5.5.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_subject_identifier">A string containing the subject_identifier, unique in the external directory service</param>
        /// <remarks>
        /// Minimum allowed role: read-only
        /// </remarks>
        public static string[] get_group_membership(Session session, string _subject_identifier)
        {
            return session.JsonRpcClient.auth_get_group_membership(session.opaque_ref, _subject_identifier);
        }
    }
}
