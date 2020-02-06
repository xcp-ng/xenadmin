/* Copyright (c) Citrix Systems, Inc. 
 * All rights reserved. 
 * 
 * Redistribution and use in source and binary forms, 
 * with or without modification, are permitted provided 
 * that the following conditions are met: 
 * 
 * *   Redistributions of source code must retain the above 
 *     copyright notice, this list of conditions and the 
 *     following disclaimer. 
 * *   Redistributions in binary form must reproduce the above 
 *     copyright notice, this list of conditions and the 
 *     following disclaimer in the documentation and/or other 
 *     materials provided with the distribution. 
 * 
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND 
 * CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, 
 * INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF 
 * MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE 
 * DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR 
 * CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, 
 * SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, 
 * BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR 
 * SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS 
 * INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, 
 * WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING 
 * NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE 
 * OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF 
 * SUCH DAMAGE.
 */

using System;
using System.Collections.Generic;
using XenAdmin.Core;
using XenAdmin.Network;

namespace XenAPI
{
    // Note that the Role object represents both the high-level roles (such as "VM Operator" etc.)
    // and their subroles, i.e., the individual calls they are allowed to make (such as "vm.create").
    public partial class Role
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public const string MR_ROLE_READ_ONLY = "read-only";
        public const string MR_ROLE_VM_OPERATOR = "vm-operator";
        public const string MR_ROLE_VM_ADMIN = "vm-admin";
        public const string MR_ROLE_VM_POWER_ADMIN = "vm-power-admin";
        public const string MR_ROLE_POOL_OPERATOR = "pool-operator";
        public const string MR_ROLE_POOL_ADMIN = "pool-admin";

        public string FriendlyName()
        {
            return FriendlyNameManager.GetFriendlyName(string.Format("Role.{0}.NameLabel", name_label.ToLowerInvariant()));
        }

        public static string FriendlyName(string role)
        {
            return FriendlyNameManager.GetFriendlyName(string.Format("Role.{0}.NameLabel", role.ToLowerInvariant()));
        }

        public string FriendlyDescription()
        {
            return FriendlyNameManager.GetFriendlyName(String.Format("Role.{0}.Description", this.name_label.ToLowerInvariant()));
        }

        public override string Name()
        {
            return name_label;
        }

        /// <summary>
        /// Currently all RBAC roles can be arranged in a rank so that each one has all the privileges of the next. Use this function for sorting based on this ordering.
        /// </summary>
        /// <returns></returns>
        public int RoleRank()
        {
            switch (name_label.ToLowerInvariant())
            {
                case MR_ROLE_READ_ONLY: return 0;
                case MR_ROLE_VM_OPERATOR: return 1;
                case MR_ROLE_VM_ADMIN: return 2;
                case MR_ROLE_VM_POWER_ADMIN: return 3;
                case MR_ROLE_POOL_OPERATOR: return 4;
                case MR_ROLE_POOL_ADMIN: return 5;
            }
            return -1;
        }

        /// <summary>
        /// logout, login_with_password
        /// </summary>
        public static readonly RbacMethodList CommonSessionApiList = new RbacMethodList(
            "session.logout", 
            "session.login_with_password"
        );

        /// <summary>
        /// add_to_other_config, destroy
        /// </summary>
        public static readonly RbacMethodList CommonTaskApiList = new RbacMethodList(
            new RbacMethod("task.add_to_other_config", "XenCenterUUID"),  // See AsyncAction.RelatedTask
            new RbacMethod("task.add_to_other_config", "applies_to"),
            new RbacMethod("task.destroy")
        );

        /// <summary>
        /// Takes a list of role objects and returns as a comma separated friendly string
        /// </summary>
        /// <param name="roles"></param>
        /// <returns></returns>
        public static string FriendlyCSVRoleList(List<Role> roles)
        {
            if (roles == null)
                return "";

            return String.Join(", ", roles.ConvertAll(r => r.FriendlyName()).ToArray());
        }


         /// <summary>
        /// Retrieves all the server RBAC roles which are able to complete all the api methods supplied. If on George or less this will return an empty list.
        /// </summary>
        /// <param name="ApiMethodsToRoleCheck">list of RbacMethods to check</param>
        /// <param name="Connection">server connection to retrieve roles from</param>
        /// <returns></returns>
        public static List<Role> ValidRoleList(RbacMethodList ApiMethodsToRoleCheck, IXenConnection Connection)
        {
            return ValidRoleList(ApiMethodsToRoleCheck, Connection, true);
        }

        /// <summary>
        /// Retrieves all the server RBAC roles which are able to complete all the api methods supplied. If on George or less this will return an empty list.
        /// </summary>
        /// <param name="ApiMethodsToRoleCheck">list of RbacMethods to check</param>
        /// <param name="Connection">server connection to retrieve roles from</param>
        /// <returns></returns>
        public static List<Role> ValidRoleList(RbacMethodList ApiMethodsToRoleCheck, IXenConnection Connection, bool debug)
        {
            List<Role> rolesAbleToCompleteAction = new List<Role>();
            if (debug)
                log.DebugFormat("Checking roles required to complete the following calls: {0}", String.Join(", ", ApiMethodsToRoleCheck.ToStringArray()));
            foreach (RbacMethod method in ApiMethodsToRoleCheck)
            {
                // For every call in the list, compile a list of the roles that can perform it, taking the intersection of the
                // roles able to perform the call in question and those in the list already.
                List<Role> rolesAbleToCompleteApiCall = ValidRoleList(method, Connection);
                if (rolesAbleToCompleteAction.Count == 0)
                {
                    rolesAbleToCompleteAction.AddRange(rolesAbleToCompleteApiCall);
                    continue;
                }
                if (rolesAbleToCompleteApiCall.Count != 0)  // zero is a bug: see Assert below
                {
                    // take intersection of existing authorized roles and this set of authorized roles
                    rolesAbleToCompleteAction.RemoveAll(delegate(Role r)
                    {
                        return !rolesAbleToCompleteApiCall.Contains(r);
                    });
                }
            }
            return rolesAbleToCompleteAction;
        }

        /// <summary>
        /// Retrieves all the server RBAC roles which are able to complete the api method supplied. If on George or less this will return an empty list.
        /// </summary>
        /// <param name="ApiMethodToRoleCheck">RbacMethod to check</param>
        /// <param name="Connection">server connection to retrieve roles from</param>
        /// <returns></returns>
        public static List<Role> ValidRoleList(RbacMethod ApiMethodToRoleCheck, IXenConnection Connection)
        {
            List<Role> rolesAbleToCompleteApiCall = new List<Role>();
            foreach (Role role in Connection.Cache.Roles)
            {
                List<Role> subroles = (List<Role>)Connection.ResolveAll<Role>(role.subroles);
                if (subroles.Find(
                    delegate(Role r)
                    {
                        return r.CanPerform(ApiMethodToRoleCheck);
                    })
                    != null)
                {
                    rolesAbleToCompleteApiCall.Add(role);
                }
            }

            // don't do this assert with simulator connections. These will always have no roles.
            if (!Connection.HostnameWithPort.EndsWith(".xml", StringComparison.InvariantCultureIgnoreCase))
            {
                // No roles able to perform API call is a bug, because Pool Admins should be able to do everything.
                // Usually caused by a typo, or by running a new action against an old server without checking.
                System.Diagnostics.Trace.Assert(rolesAbleToCompleteApiCall.Count > 0, String.Format("No roles able to perform API call {0}", ApiMethodToRoleCheck));
            }
            return rolesAbleToCompleteApiCall;
        }

        /// <summary>
        /// Retrieves all the server RBAC roles which are able to complete the api method supplied. If on George or less this will return an empty list.
        /// </summary>
        /// <param name="ApiMethodToRoleCheck">object.method</param>
        /// <param name="Connection">server connection to retrieve roles from</param>
        /// <returns></returns>
        public static List<Role> ValidRoleList(string ApiMethodToRoleCheck, IXenConnection Connection)
        {
            return ValidRoleList(new RbacMethod(ApiMethodToRoleCheck), Connection);
        }

        /// <summary>
        /// Can the main session on this connection already perform all the API methods? If on George or less this will return false.
        /// Also return the list of valid roles.
        /// </summary>
        /// <param name="apiMethodsToRoleCheck">The methods to check</param>
        /// <param name="connection">The connection on which to perform the methods</param>
        /// <param name="validRoleList">The list of roles which can perform all the methods</param>
        public static bool CanPerform(RbacMethodList apiMethodsToRoleCheck, IXenConnection connection, out List<Role> validRoleList, bool debug)
        {
            if (!connection.IsConnected)
            {
                validRoleList = new List<Role>();
                return false;
            }
            else
                validRoleList = ValidRoleList(apiMethodsToRoleCheck, connection, debug);
            
            if (connection.Session != null && connection.Session.IsLocalSuperuser)
                return true;

            foreach (Role role in validRoleList)
            {
                if (connection.Session != null && connection.Session.Roles != null && connection.Session.Roles.Contains(role))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Can the main session on this connection already perform all the API methods? If on George or less this will return false.
        /// Also return the list of valid roles.
        /// </summary>
        /// <param name="apiMethodsToRoleCheck">The methods to check</param>
        /// <param name="connection">The connection on which to perform the methods</param>
        /// <param name="validRoleList">The list of roles which can perform all the methods</param>
        public static bool CanPerform(RbacMethodList apiMethodsToRoleCheck, IXenConnection connection, out List<Role> validRoleList)
        {
            return CanPerform(apiMethodsToRoleCheck, connection, out validRoleList, true);
        }

        /// <summary>
        /// Can the main session on this connection already perform all the API methods? If on George or less this will return false.
        /// </summary>
        /// <param name="apiMethodsToRoleCheck">The methods to check</param>
        /// <param name="connection">The connection on which to perform the methods</param>
        public static bool CanPerform(RbacMethodList apiMethodsToRoleCheck, IXenConnection connection)
        {
            List<Role> validRoleList;
            return CanPerform(apiMethodsToRoleCheck, connection, out validRoleList);
        }

        /// <summary>
        /// Can the main session on this connection already perform all the API methods? If on George or less this will return false.
        /// </summary>
        /// <param name="apiMethodsToRoleCheck">The methods to check</param>
        /// <param name="connection">The connection on which to perform the methods</param>
        public static bool CanPerform(RbacMethodList apiMethodsToRoleCheck, IXenConnection connection, bool debug)
        {
            List<Role> validRoleList;
            return CanPerform(apiMethodsToRoleCheck, connection, out validRoleList, debug);
        }

        /// <summary>
        /// Can this subrole perform this API call?
        /// </summary>
        /// <param name="rbacMethod">The API call which we want to perform</param>
        /// <returns></returns>
        private bool CanPerform(RbacMethod rbacMethod)
        {
            // Does the method name match?
            if (name_label == rbacMethod.Method)
                return true;
            
            // Is the call a hash table modification, and if so, does the
            // more specific name match?
            if (!String.IsNullOrEmpty(rbacMethod.Key))
            {
                string whole = rbacMethod.ToString();
                if (name_label.EndsWith("*"))  // e.g. vm.add_to_other_config/key:Foo*
                {
                    string stripped_name = name_label.TrimEnd('*');
                    if (whole.StartsWith(stripped_name))
                        return true;
                }
                else  // e.g. vm.add_to_other_config/key:Foo
                {
                    if (name_label == whole)
                        return true;
                }
            }

            return false;
        }

       

        #region XenObjectComparable<Role> Members

        public override int CompareTo(Role other)
        {
            int rank = RoleRank();
            int otherRank = other.RoleRank();
            return rank > otherRank ? 1 : rank < otherRank ? -1 : 0;
        }

        public override bool Equals(object obj)
        {
            Role r = obj as Role;
            if (r != null)
            {
                return r.opaque_ref == this.opaque_ref;
            }
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return opaque_ref.GetHashCode();
        }

        #endregion
    }
}
