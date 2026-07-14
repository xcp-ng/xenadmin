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
using System.Collections.Generic;
using System.Linq;
using System.Net;
#if (NET8_0_OR_GREATER)
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
#endif
using System.Net.Security;
using Newtonsoft.Json;


namespace XenAPI
{
    public partial class Session : XenObject<Session>
    {
        public const int STANDARD_TIMEOUT = 24 * 60 * 60 * 1000;

        /// <summary>
        /// The default HTTP UserAgent for each request.
        /// </summary>
        public static readonly string DefaultUserAgent = $"XenServer.NET/@SDK_VERSION@";

        #region Constructors

        /// <exception cref="ArgumentNullException">Thrown if 'client' is null</exception>
        public Session(JsonRpcClient client)
        {
            JsonRpcClient = client ?? throw new ArgumentNullException(nameof(client));
        }

        public Session(string url) :
            this(new JsonRpcClient(url))
        {
        }

        public Session(string host, int port)
            : this(GetUrl(host, port))
        {
        }

        public Session(string url, string opaqueRef)
            : this(url)
        {
            opaque_ref = opaqueRef;
            SetupSessionDetails();
        }

        /// <summary>
        /// Create a new Session instance, using the given instance. The connection details
        /// and Xen-API session handle will be copied from the given instance, but a new
        /// connection will be created. Use this if you want a duplicate connection to a host,
        /// for example when you need to cancel an operation that is blocking the primary connection.
        /// </summary>
        public Session(Session session)
        {
            opaque_ref = session.opaque_ref;
            APIVersion = session.APIVersion;

            //in the following do not copy over the ConnectionGroupName

            if (APIVersion == API_Version.API_2_6 || APIVersion >= API_Version.API_2_8)
            {
                JsonRpcClient = new JsonRpcClient(session.Url)
                {
                    JsonRpcVersion = session.JsonRpcClient.JsonRpcVersion,
                    UserAgent = session.JsonRpcClient.UserAgent,
                    KeepAlive = session.JsonRpcClient.KeepAlive,
                    WebProxy = session.JsonRpcClient.WebProxy,
                    Timeout = session.JsonRpcClient.Timeout,
                    ProtocolVersion = session.JsonRpcClient.ProtocolVersion,
                    Expect100Continue = session.JsonRpcClient.Expect100Continue,
                    AllowAutoRedirect = session.JsonRpcClient.AllowAutoRedirect,
                    PreAuthenticate = session.JsonRpcClient.PreAuthenticate,
                    Cookies = session.JsonRpcClient.Cookies,
                    ServerCertificateValidationCallback = session.JsonRpcClient.ServerCertificateValidationCallback
                };
            }

            //copy AD details
            IsLocalSuperuser = session.IsLocalSuperuser;
            SessionSubject = session.SessionSubject;
            UserSid = session.UserSid;
            Permissions = session.Permissions;
        }

        #endregion

        private static string GetUrl(string hostname, int port)
        {
            return $"{(port == 8080 || port == 80 ? "http" : "https")}://{hostname}:{port}";
        }

        private void SetupSessionDetails()
        {
            var pools = Pool.get_all_records(this);

            if (pools.Values.Count > 0)
            {
                var pool = pools.Values.First();
                Host host = Host.get_record(this, pool.master);
                APIVersion = Helper.GetAPIVersion(host.API_version_major, host.API_version_minor);
            }

            //the SDK cannot connect to servers with API < 2.5 because JsonRPC was not available

            if (APIVersion == API_Version.API_2_6)
                JsonRpcClient.JsonRpcVersion = JsonRpcVersion.v1;
            else if (APIVersion >= API_Version.API_2_8)
                JsonRpcClient.JsonRpcVersion = JsonRpcVersion.v2;

            IsLocalSuperuser = get_is_local_superuser(this, opaque_ref);

            if (!IsLocalSuperuser)
            {
                SessionSubject = get_subject(this, opaque_ref);
                UserSid = get_auth_user_sid(this, opaque_ref);

                // Cache the details of this user to avoid making server calls later
                UserDetails.UpdateDetails(UserSid, this);
            }

            Permissions = get_rbac_permissions(this, opaque_ref);
        }

        public override void UpdateFrom(Session update)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #region Properties

        public API_Version APIVersion { get; private set;  } = API_Version.UNKNOWN;

        public object Tag { get; set; }

        /// <summary>
        /// Retrieves the current users details from the UserDetails map. These values are only updated when a new session is created.
        /// </summary>
        public virtual UserDetails CurrentUserDetails => UserSid == null ? null : UserDetails.Sid_To_UserDetails[UserSid];

        public JsonRpcClient JsonRpcClient { get; }

        public string Url => JsonRpcClient.Url;

        /// <summary>
        /// The WebProxy to use for each HTTP request.
        /// </summary>
        public IWebProxy Proxy
        {
            get => JsonRpcClient.WebProxy;
            set => JsonRpcClient.WebProxy = value;
        }
        
        /// <summary>
        /// The UserAgent to use for each HTTP request. If set to null or empty the DefaultUserAgent will be used.
        /// </summary>
        public string UserAgent
        {
            get => JsonRpcClient.UserAgent;
            set => JsonRpcClient.UserAgent = value;
        }

        public string ConnectionGroupName
        {
            get => JsonRpcClient.ConnectionGroupName;
            set => JsonRpcClient.ConnectionGroupName = value;
        }

        public int Timeout
        {
            get => JsonRpcClient.Timeout;
            set => JsonRpcClient.Timeout = value;
        }

#if (NET8_0_OR_GREATER)
        public Func<HttpRequestMessage, X509Certificate2, X509Chain, SslPolicyErrors, bool> ServerCertificateValidationCallback
        {
            get => JsonRpcClient.ServerCertificateValidationCallback;
            set => JsonRpcClient.ServerCertificateValidationCallback = value;
        }
#else
        public RemoteCertificateValidationCallback ServerCertificateValidationCallback
        {
            get => JsonRpcClient.ServerCertificateValidationCallback;
            set => JsonRpcClient.ServerCertificateValidationCallback = value;
        }
#endif

        public ICredentials Credentials => JsonRpcClient.WebProxy?.Credentials;

        /// <summary>
        /// Optional headers in name-value pairs to be passed in the HttpWebRequests. The
        /// default value is null. This property can be set by the implementing code before
        /// each request. It is automatically reset to null once the request has been sent.
        /// </summary>
        public Dictionary<string, string> RequestHeaders
        {
            set => JsonRpcClient.RequestHeaders = value;
            get => JsonRpcClient.RequestHeaders;
        }

        /// <summary>
        /// Exposes the headers returned in the HttpWebResponses in name-value pairs.
        /// This property is set once a response is received. The values are comma
        /// separated strings of header values stored in a header.
        /// It returns an empty dictionary if no headers are found.
        /// </summary>
        public Dictionary<string, string> ResponseHeaders => JsonRpcClient.ResponseHeaders;

        /// <summary>
        /// Always true before API version 1.6.
        /// Filled in after successful session_login_with_password for 1.6 or newer connections
        /// </summary>
        public virtual bool IsLocalSuperuser { get; private set; }

        /// <summary>
        /// The OpaqueRef for the Subject under whose authority the current user is logged in;
        /// may correspond to either a group or a user.
        /// Null if IsLocalSuperuser is true.
        /// </summary>
        [JsonConverter(typeof(XenRefConverter<Subject>))]
        public XenRef<Subject> SessionSubject { get; private set; }

        /// <summary>
        /// The Active Directory SID of the currently logged-in user.
        /// Null if IsLocalSuperuser is true.
        /// </summary>
        public string UserSid { get; private set; }

        /// <summary>
        /// All permissions associated with the session at the time of log in.
        /// This is the list xapi uses until the session is logged out;
        /// even if the permitted roles change on the server side, they don't apply until the next session.
        /// </summary>
        public string[] Permissions { get; private set; }

        #endregion

        public string[] GetSystemMethods()
        {
            return JsonRpcClient.system_list_methods();
        }

        public static Session get_record(Session session, string sessionOpaqueRef)
        {
            return session.JsonRpcClient.session_get_record(session.opaque_ref, sessionOpaqueRef);
        }

        public void login_with_password(string username, string password)
        {
            opaque_ref = JsonRpcClient.session_login_with_password(username, password);
            SetupSessionDetails();
        }

        public void login_with_password(string username, string password, string version)
        {
            try
            {
                opaque_ref = JsonRpcClient.session_login_with_password(username, password, version);
                SetupSessionDetails();
            }
            catch (Failure exn)
            {
                if (exn.ErrorDescription[0] == Failure.MESSAGE_PARAMETER_COUNT_MISMATCH)
                {
                    // Call the 1.1 version instead.
                    login_with_password(username, password);
                }
                else
                {
                    throw;
                }
            }
        }

        public void login_with_password(string username, string password, string version, string originator)
        {
            try
            {
                opaque_ref = JsonRpcClient.session_login_with_password(username, password, version, originator);
                SetupSessionDetails();
            }
            catch (Failure exn)
            {
                if (exn.ErrorDescription[0] == Failure.MESSAGE_PARAMETER_COUNT_MISMATCH)
                {
                    // Call the pre-2.0 version instead.
                    login_with_password(username, password, version);
                }
                else
                {
                    throw;
                }
            }
        }

        [Obsolete("Use method login_with_password(string username, string password, string version) instead")]
        public void login_with_password(string username, string password, API_Version version)
        {
            login_with_password(username, password, Helper.APIVersionString(version));
        }

        public void slave_local_login_with_password(string username, string password)
        {
            opaque_ref = JsonRpcClient.session_slave_local_login_with_password(username, password);
            //assume the latest API
            APIVersion = API_Version.LATEST;
        }

        public void logout()
        {
            session_logout(this, opaque_ref);
            opaque_ref = null;
        }
        
        [Obsolete("Use static method session_logout(Session session, string opaqueRef) instead")]
        public void logout(Session session2)
        {
            logout(session2.opaque_ref);
            session2.opaque_ref = null;
        }

        [Obsolete("Use static method session_logout(Session session, string opaqueRef) instead")]
        public void logout(string self)
        {
            if (self == null)
                return;

            JsonRpcClient.session_logout(self);
        }

        public static void session_logout(Session session, string opaqueRef)
        {
            session.JsonRpcClient.session_logout(opaqueRef);
        }

        public void local_logout()
        {
            session_local_logout(this, opaque_ref);
            opaque_ref = null;
        }

        [Obsolete("Use static method session_local_logout(Session session, string opaqueRef) instead")]
        public void local_logout(Session session2)
        {
            local_logout(session2.opaque_ref);
            session2.opaque_ref = null;
        }

        [Obsolete("Use static method session_local_logout(Session session, string opaqueRef) instead")]
        public void local_logout(string opaqueRef)
        {
            if (opaqueRef == null)
                return;

            JsonRpcClient.session_local_logout(opaqueRef);
        }

        public static void session_local_logout(Session session, string opaqueRef)
        {
            session.JsonRpcClient.session_local_logout(opaqueRef);
        }

        [Obsolete("Use static method Session.change_password instead")]
        public void change_password(string oldPassword, string newPassword)
        {
            change_password(this, oldPassword, newPassword);
        }

        public static void change_password(Session session, string oldPassword, string newPassword)
        {
            session.JsonRpcClient.session_change_password(session.opaque_ref, oldPassword, newPassword);
        }

        [Obsolete("Use static method Session.get_this_host instead")]
        public string get_this_host()
        {
            return get_this_host(this, opaque_ref);
        }

        public static string get_this_host(Session session, string self)
        {
            return session.JsonRpcClient.session_get_this_host(session.opaque_ref, self ?? "");
        }

        [Obsolete("Use static method Session.get_this_user instead")]
        public string get_this_user()
        {
            return get_this_user(this, opaque_ref);
        }

        public static string get_this_user(Session session, string self)
        {
            return session.JsonRpcClient.session_get_this_user(session.opaque_ref, self ?? "");
        }

        [Obsolete("Use static method Session.get_is_local_superuser instead")]
        public bool get_is_local_superuser()
        {
            return get_is_local_superuser(this, opaque_ref);
        }

        public static bool get_is_local_superuser(Session session, string self)
        {
            return session.JsonRpcClient.session_get_is_local_superuser(session.opaque_ref, self ?? "");
        }

        public static string[] get_rbac_permissions(Session session, string self)
        {
            return session.JsonRpcClient.session_get_rbac_permissions(session.opaque_ref, self ?? "");
        }

        [Obsolete("Use static method Session.get_last_active instead")]
        public DateTime get_last_active()
        {
            return get_last_active(this, opaque_ref);
        }

        public static DateTime get_last_active(Session session, string self)
        {
            return session.JsonRpcClient.session_get_last_active(session.opaque_ref, self ?? "");
        }

        [Obsolete("Use static method Session.get_pool instead")]
        public bool get_pool()
        {
            return get_pool(this, opaque_ref);
        }

        public static bool get_pool(Session session, string self)
        {
            return session.JsonRpcClient.session_get_pool(session.opaque_ref, self ?? "");
        }

        [Obsolete("Use static method Session.get_subject instead")]
        public XenRef<Subject> get_subject()
        {
            return get_subject(this, opaque_ref);
        }

        public static XenRef<Subject> get_subject(Session session, string self)
        {
            return session.JsonRpcClient.session_get_subject(session.opaque_ref, self ?? "");
        }

        [Obsolete("Use static method Session.get_auth_user_sid instead")]
        public string get_auth_user_sid()
        {
            return get_auth_user_sid(this, opaque_ref);
        }

        public static string get_auth_user_sid(Session session, string self)
        {
            return session.JsonRpcClient.session_get_auth_user_sid(session.opaque_ref, self ?? "");
        }

        [Obsolete("Use static method Session.get_all_subject_identifiers instead")]
        public string[] get_all_subject_identifiers()
        {
            return get_all_subject_identifiers(this);
        }

        public static string[] get_all_subject_identifiers(Session session)
        {
            return session.JsonRpcClient.session_get_all_subject_identifiers(session.opaque_ref);
        }

        [Obsolete("Use static method Session.async_get_all_subject_identifiers instead")]
        public XenRef<Task> async_get_all_subject_identifiers()
        {
            return async_get_all_subject_identifiers(this);
        }

        public static XenRef<Task> async_get_all_subject_identifiers(Session session)
        {
            return session.JsonRpcClient.async_session_get_all_subject_identifiers(session.opaque_ref);
        }

        [Obsolete("Use static method Session.logout_subject_identifier instead")]
        public void logout_subject_identifier(string subjectIdentifier)
        {
            logout_subject_identifier(this, subjectIdentifier);
        }

        public static void logout_subject_identifier(Session session, string subjectIdentifier)
        {
            session.JsonRpcClient.session_logout_subject_identifier(session.opaque_ref, subjectIdentifier);
        }

        [Obsolete("Use static method Session.async_logout_subject_identifier instead")]
        public XenRef<Task> async_logout_subject_identifier(string subjectIdentifier)
        {
            return async_logout_subject_identifier(this, subjectIdentifier);
        }

        public static XenRef<Task> async_logout_subject_identifier(Session session, string subjectIdentifier)
        {
            return session.JsonRpcClient.async_session_logout_subject_identifier(session.opaque_ref, subjectIdentifier);
        }

        [Obsolete("Use static method Session.get_other_config instead")]
        public Dictionary<string, string> get_other_config()
        {
            return get_other_config(this, opaque_ref);
        }

        public static Dictionary<string, string> get_other_config(Session session, string self)
        {
            return session.JsonRpcClient.session_get_other_config(session.opaque_ref, self ?? "");
        }

        [Obsolete("Use static method Session.set_other_config instead")]
        public void set_other_config(Dictionary<string, string> otherConfig)
        {
            set_other_config(this, opaque_ref, otherConfig);
        }

        public static void set_other_config(Session session, string self, Dictionary<string, string> otherConfig)
        {
            session.JsonRpcClient.session_set_other_config(session.opaque_ref, self ?? "", otherConfig);
        }

        [Obsolete("Use static method Session.add_to_other_config instead")]
        public void add_to_other_config(string key, string value)
        {
            add_to_other_config(this, opaque_ref, key, value);
        }

        public static void add_to_other_config(Session session, string self, string key, string value)
        {
            session.JsonRpcClient.session_add_to_other_config(session.opaque_ref, self ?? "", key ?? "", value ?? "");
        }

        [Obsolete("Use static method Session.remove_from_other_config instead")]
        public void remove_from_other_config(string key)
        {
            remove_from_other_config(this, opaque_ref, key);
        }

        public static void remove_from_other_config(Session session, string self, string key)
        {
            session.JsonRpcClient.session_remove_from_other_config(session.opaque_ref, self ?? "", key ?? "");
        }
    }
}
