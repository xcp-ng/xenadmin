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
using System.Text;
using XenAdmin;
using XenAdmin.Core;
using XenAdmin.Network;


namespace XenAPI
{
    public partial class Failure : Exception
    {
        public const string CANNOT_EVACUATE_HOST = "CANNOT_EVACUATE_HOST";
        public const string DEVICE_ALREADY_DETACHED = "DEVICE_ALREADY_DETACHED";
        public const string HANDLE_INVALID = "HANDLE_INVALID";
        public const string HA_NO_PLAN = "HA_NO_PLAN";
        public const string HA_OPERATION_WOULD_BREAK_FAILOVER_PLAN = "HA_OPERATION_WOULD_BREAK_FAILOVER_PLAN";
        public const string HOST_IS_SLAVE = "HOST_IS_SLAVE";
        public const string HOST_OFFLINE = "HOST_OFFLINE";
        public const string HOST_STILL_BOOTING = "HOST_STILL_BOOTING";
        public const string NO_HOSTS_AVAILABLE = "NO_HOSTS_AVAILABLE";
        public const string PATCH_ALREADY_EXISTS = "PATCH_ALREADY_EXISTS";
        public const string PATCH_APPLY_FAILED = "PATCH_APPLY_FAILED";
        public const string SESSION_AUTHENTICATION_FAILED = "SESSION_AUTHENTICATION_FAILED";
        public const string SESSION_INVALID = "SESSION_INVALID";
        public const string SR_HAS_NO_PBDS = "SR_HAS_NO_PBDS";
        public const string VM_BAD_POWER_STATE = "VM_BAD_POWER_STATE";
        public const string VM_REQUIRES_SR = "VM_REQUIRES_SR";
        public const string VM_REQUIRES_NETWORK = "VM_REQUIRES_NETWORK";
        public const string VM_REQUIRES_GPU = "VM_REQUIRES_GPU";
        public const string VM_MISSING_PV_DRIVERS = "VM_MISSING_PV_DRIVERS";
        public const string HOST_NOT_ENOUGH_FREE_MEMORY = "HOST_NOT_ENOUGH_FREE_MEMORY";
        public const string SR_BACKEND_FAILURE_72 = "SR_BACKEND_FAILURE_72";
        public const string SR_BACKEND_FAILURE_73 = "SR_BACKEND_FAILURE_73";
        public const string SR_BACKEND_FAILURE_107 = "SR_BACKEND_FAILURE_107";
        public const string SR_BACKEND_FAILURE_111 = "SR_BACKEND_FAILURE_111";
        public const string SR_BACKEND_FAILURE_112 = "SR_BACKEND_FAILURE_112";
        public const string SR_BACKEND_FAILURE_113 = "SR_BACKEND_FAILURE_113";
        public const string SR_BACKEND_FAILURE_114 = "SR_BACKEND_FAILURE_114";
        public const string SR_BACKEND_FAILURE_140 = "SR_BACKEND_FAILURE_140";
        public const string SR_BACKEND_FAILURE_222 = "SR_BACKEND_FAILURE_222";
        public const string SR_BACKEND_FAILURE_225 = "SR_BACKEND_FAILURE_225";
        public const string SR_BACKEND_FAILURE_454 = "SR_BACKEND_FAILURE_454";
        public const string SUBJECT_CANNOT_BE_RESOLVED = "SUBJECT_CANNOT_BE_RESOLVED";
        public const string OBJECT_NO_LONGER_EXISTS = "OBJECT_NOLONGER_EXISTS";
        public const string PERMISSION_DENIED = "PERMISSION_DENIED";
        public const string RBAC_PERMISSION_DENIED_FRIENDLY = "RBAC_PERMISSION_DENIED_FRIENDLY";
        public const string RBAC_PERMISSION_DENIED = "RBAC_PERMISSION_DENIED";
        public const string LICENSE_CHECKOUT_ERROR = "LICENSE_CHECKOUT_ERROR";
        public const string VDI_IN_USE = "VDI_IN_USE";
        public const string AUTH_ENABLE_FAILED = "AUTH_ENABLE_FAILED";
        public const string POOL_AUTH_ENABLE_FAILED_WRONG_CREDENTIALS = "POOL_AUTH_ENABLE_FAILED_WRONG_CREDENTIALS";
        public const string HOST_UNKNOWN_TO_MASTER = "HOST_UNKNOWN_TO_MASTER";
        public const string VM_HAS_VGPU = "VM_HAS_VGPU";
        public const string VM_HAS_PCI_ATTACHED = "VM_HAS_PCI_ATTACHED";
        public const string OUT_OF_SPACE = "OUT_OF_SPACE";
        public const string PVS_SITE_CONTAINS_RUNNING_PROXIES = "PVS_SITE_CONTAINS_RUNNING_PROXIES";
        public const string VM_LACKS_FEATURE = "VM_LACKS_FEATURE";
        public const string VM_LACKS_FEATURE_SUSPEND = "VM_LACKS_FEATURE_SUSPEND";
        public const string VM_FAILED_SHUTDOWN_ACKNOWLEDGMENT = "VM_FAILED_SHUTDOWN_ACKNOWLEDGMENT";
        public const string OTHER_OPERATION_IN_PROGRESS = "OTHER_OPERATION_IN_PROGRESS";
        public const string PATCH_ALREADY_APPLIED = "PATCH_ALREADY_APPLIED";
        public const string UPDATE_ALREADY_APPLIED = "UPDATE_ALREADY_APPLIED";
        public const string UPDATE_ALREADY_EXISTS = "UPDATE_ALREADY_EXISTS";

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Changes a techy RBAC Failure into a pretty print one that shows the roles that would be required to complete the failed action.
        /// Requires context such as the the connection and current session to populate these fields.
        /// </summary>
        /// <param name="failure">The Failure to update</param>
        /// <param name="Connection">The current connection</param>
        /// <param name="Session">The current session, passed separately because it could be an elevated session, different to the heartbeat</param>
        public static void ParseRBACFailure(Failure failure, IXenConnection Connection, Session Session)
        {
            List<Role> authRoles = Role.ValidRoleList(failure.ErrorDescription[1], Connection);
            failure.ErrorDescription[0] = Failure.RBAC_PERMISSION_DENIED_FRIENDLY;
            // Current Role(s)
            failure.ErrorDescription[1] = Session.FriendlyRoleDescription();
            // Authorized roles
            failure.ErrorDescription[2] = Role.FriendlyCSVRoleList(authRoles);
            failure.Setup();
        }

        /// <summary>
        /// This overload is for the special case of us doing an action over multiple connections. Assumes the role requirement is the same across all conections.
        /// </summary>
        /// <param name="failure">The Failure to update</param>
        /// <param name="Sessions">One session per connection, the ones used to perform the action. Passed separately because they could be elevated sessions, different to the heartbeat</param>
        public static void ParseRBACFailure(Failure failure, Session[] Sessions)
        {
            List<Role> authRoles = Role.ValidRoleList(failure.ErrorDescription[1], Sessions[0].Connection);
            failure.ErrorDescription[0] = Failure.RBAC_PERMISSION_DENIED_FRIENDLY;
            // Current Role(s)
            StringBuilder sb = new StringBuilder();
            foreach (Session s in Sessions)
            {
                sb.Append(string.Format(Messages.ROLE_ON_CONNECTION, s.FriendlyRoleDescription(), Helpers.GetName(s.Connection).Ellipsise(50)));
                sb.Append(", ");
            }
            string output = sb.ToString();
            // remove trailing comma and space
            output = output.Substring(0, output.Length - 2);
            failure.ErrorDescription[1] = output;
            // Authorized roles
            failure.ErrorDescription[2] = Role.FriendlyCSVRoleList(authRoles);
            failure.Setup();
        }
    }
}