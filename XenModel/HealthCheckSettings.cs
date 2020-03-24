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
using XenAPI;
using XenAdmin.Network;
using XenAdmin.Core;
using System.Globalization;

namespace XenAdmin.Model
{
    public enum HealthCheckStatus
    {
        Disabled, Enabled, Undefined
    }

    public enum DiagnosticAlertSeverity
    {
        Info, Warning, Error
    }

    public class HealthCheckSettings
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public HealthCheckStatus Status;
        public int IntervalInDays, TimeOfDay, RetryInterval;
        public DayOfWeek DayOfWeek;
        public string UploadTokenSecretUuid;
        public string NewUploadRequest;
        public string UserNameSecretUuid;
        public string PasswordSecretUuid;
        public string LastSuccessfulUpload;
        public string LastFailedUpload;
        public string DiagnosticTokenSecretUuid;
        public string UploadUuid;
        public DiagnosticAlertSeverity ReportAnalysisSeverity;
        public int ReportAnalysisIssuesDetected;
        public string ReportAnalysisUploadUuid;
        public string ReportAnalysisUploadTime;
        
        public const int DEFAULT_INTERVAL_IN_DAYS = 14;
        public const int DEFAULT_RETRY_INTERVAL = 7; // in days
        public const int UPLOAD_REQUEST_VALIDITY_INTERVAL = 30; // in minutes

        public const string STATUS = "Enrollment";
        public const string INTERVAL_IN_DAYS = "Schedule.IntervalInDays";
        public const string DAY_OF_WEEK = "Schedule.DayOfWeek";
        public const string TIME_OF_DAY = "Schedule.TimeOfDay";
        public const string RETRY_INTERVAL = "Schedule.RetryInterval";
        public const string UPLOAD_TOKEN_SECRET = "UploadToken.Secret";
        public const string UPLOAD_CREDENTIAL_USER_SECRET = "User.Secret";
        public const string UPLOAD_CREDENTIAL_PASSWORD_SECRET = "Password.Secret";
        public const string UPLOAD_LOCK = "UploadLock";
        public const string LAST_SUCCESSFUL_UPLOAD = "LastSuccessfulUpload";
        public const string LAST_FAILED_UPLOAD = "LastFailedUpload";
        public const string NEW_UPLOAD_REQUEST = "NewUploadRequest";
        public const string HEALTH_CHECK_PIPE = "HealthCheckServicePipe";
        public const string HEALTH_CHECK_PIPE_END_MESSAGE = "HealthCheckServicePipe";
        public const string UPLOAD_UUID = "UploadUuid";
        public const string PROXY_SETTINGS = "ProxySettings";
        public const string XENCENTER_METADATA = "XenCenterMetadata";
        public const string REPORT_TIME_PLACEHOLDER = "@HealthCheckReportTime@";
        public const string DIAGNOSTIC_TOKEN_SECRET = "DiagnosticToken.Secret";
        public const string REPORT_ANALYSIS_SEVERITY = "ReportAnalysis.Severity";
        public const string REPORT_ANALYSIS_ISSUES_DETECTED = "ReportAnalysis.IssuesDetected";
        public const string REPORT_ANALYSIS_UPLOAD_UUID = "ReportAnalysis.UploadUuid";
        public const string REPORT_ANALYSIS_UPLOAD_TIME = "ReportAnalysis.UploadTime";


        private const string REPORT_LINK_DOMAIN_NAME = "";
        private const string REPORT_LINK_PATH = "AutoSupport/analysis/upload_overview";

        public HealthCheckSettings(Dictionary<string, string> config)
        {
            Status = config == null || !config.ContainsKey(STATUS)
                           ? HealthCheckStatus.Undefined
                           : (BoolKey(config, STATUS) ? HealthCheckStatus.Enabled : HealthCheckStatus.Disabled);
            IntervalInDays = IntKey(config, INTERVAL_IN_DAYS, DEFAULT_INTERVAL_IN_DAYS);
            if (!Enum.TryParse(Get(config, DAY_OF_WEEK), out DayOfWeek))
                DayOfWeek = (DayOfWeek) GetDefaultDay();
            TimeOfDay = IntKey(config, TIME_OF_DAY, GetDefaultTime());
            RetryInterval = IntKey(config, RETRY_INTERVAL, DEFAULT_RETRY_INTERVAL);
            UploadTokenSecretUuid = Get(config, UPLOAD_TOKEN_SECRET);
            DiagnosticTokenSecretUuid = Get(config, DIAGNOSTIC_TOKEN_SECRET);
            NewUploadRequest = Get(config, NEW_UPLOAD_REQUEST);
            UserNameSecretUuid = Get(config, UPLOAD_CREDENTIAL_USER_SECRET);
            PasswordSecretUuid = Get(config, UPLOAD_CREDENTIAL_PASSWORD_SECRET);
            LastSuccessfulUpload = Get(config, LAST_SUCCESSFUL_UPLOAD);
            LastFailedUpload = Get(config, LAST_FAILED_UPLOAD);
            UploadUuid = Get(config, UPLOAD_UUID);
            ReportAnalysisSeverity = StringToDiagnosticAlertSeverity(Get(config, REPORT_ANALYSIS_SEVERITY));
            ReportAnalysisIssuesDetected = IntKey(config, REPORT_ANALYSIS_ISSUES_DETECTED, 0);
            ReportAnalysisUploadUuid = Get(config, REPORT_ANALYSIS_UPLOAD_UUID);
            ReportAnalysisUploadTime = Get(config, REPORT_ANALYSIS_UPLOAD_TIME);
        }

        public Dictionary<string, string> ToDictionary(Dictionary<string, string> baseDictionary)
        {
            var newConfig = baseDictionary == null ? new Dictionary<string, string>() : new Dictionary<string, string>(baseDictionary);
            newConfig[STATUS] = Status == HealthCheckStatus.Enabled ? "true" : "false";
            newConfig[INTERVAL_IN_DAYS] = IntervalInDays.ToString();
            var day = (int) DayOfWeek;
            newConfig[DAY_OF_WEEK] = day.ToString();
            newConfig[TIME_OF_DAY] = TimeOfDay.ToString();
            newConfig[RETRY_INTERVAL] = RetryInterval.ToString();
            newConfig[UPLOAD_TOKEN_SECRET] = UploadTokenSecretUuid;
            newConfig[DIAGNOSTIC_TOKEN_SECRET] = DiagnosticTokenSecretUuid;
            newConfig[NEW_UPLOAD_REQUEST] = NewUploadRequest;
            return newConfig;
        }

        public int IntervalInWeeks { get { return IntervalInDays / 7; } }

        public string StatusDescription
        {
            get
            {
                return Status == HealthCheckStatus.Enabled
                           ? ReportAnalysisStatus
                           : Messages.HEALTHCHECK_STATUS_NOT_ENROLLED;
            }
        }

        public bool CanRequestNewUpload
        {
            get
            {
                if (Status != HealthCheckStatus.Enabled)
                    return false;
                var uploadRequestExpiryTime = NewUploadRequestTime.AddMinutes(UPLOAD_REQUEST_VALIDITY_INTERVAL);
                return DateTime.Compare(uploadRequestExpiryTime, DateTime.UtcNow) < 0;
            }
        }

        public DateTime NewUploadRequestTime
        {
            get
            {
                if (!string.IsNullOrEmpty(NewUploadRequest))
                {
                    try
                    {
                        return StringToDateTime(NewUploadRequest);
                    }
                    catch (Exception exn)
                    {
                        log.Error("Exception while parsing NewUploadRequest", exn);
                    }
                }
                return DateTime.MinValue;
            }
        }

        public string ReportAnalysisStatus
        {
            get
            {
                if (HasAnalysisResult)
                {
                    switch (ReportAnalysisIssuesDetected)
                    {
                        case 0:
                            return Messages.HEALTHCHECK_STATUS_NO_ISSUES_FOUND; 
                        case 1:
                            return Messages.HEALTHCHECK_STATUS_ONE_ISSUE_FOUND; 
                        default:
                            return String.Format(Messages.HEALTHCHECK_STATUS_ISSUES_FOUND, ReportAnalysisIssuesDetected); 
                    }
                }
                return HasUpload ? Messages.HEALTHCHECK_STATUS_NOT_AVAILABLE_YET : Messages.HEALTHCHECK_STATUS_NO_UPLOAD_YET;
            }
        }

        public string GetReportAnalysisLink(string domainName)
        {
            return string.Format("{0}/{1}/{2}", string.IsNullOrEmpty(domainName) ? REPORT_LINK_DOMAIN_NAME : domainName, REPORT_LINK_PATH, ReportAnalysisUploadUuid);
        }

        public bool HasUpload
        {
            get { return !string.IsNullOrEmpty(UploadUuid); }
        }

        public bool HasAnalysisResult
        {
            get { return HasUpload && ReportAnalysisUploadUuid == UploadUuid; }
        }

        public bool HasOldAnalysisResult
        {
            get
            {
                return !HasAnalysisResult && !string.IsNullOrEmpty(ReportAnalysisUploadUuid) &&
                       !string.IsNullOrEmpty(ReportAnalysisUploadTime);
            }
        }

        public DateTime LastFailedUploadTime
        {
            get
            {
                DateTime lastFailedUploadTime;
                return !string.IsNullOrEmpty(LastFailedUpload) && TryParseStringToDateTime(LastFailedUpload, out lastFailedUploadTime)
                    ? lastFailedUploadTime
                    : DateTime.MinValue;
            }
        }

        public DateTime LastSuccessfulUploadTime
        {
            get
            {
                DateTime lastSuccessfulUploadTime;
                return !string.IsNullOrEmpty(LastSuccessfulUpload) && TryParseStringToDateTime(LastSuccessfulUpload, out lastSuccessfulUploadTime)
                    ? lastSuccessfulUploadTime
                    : DateTime.MinValue;
            }
        }

        #region Helper functions
        private static T Get<T>(Dictionary<string, T> d, string k) where T : class
        {
            return d != null && d.ContainsKey(k) ? d[k] : null;
        }

        private static bool BoolKey(Dictionary<string, string> d, string k)
        {
            string v = Get(d, k);
            return v == null ? false : v == "true";
        }

        private static int IntKey(Dictionary<string, string> d, string k, int def)
        {
            int result;
            string s = Get(d, k);
            return s != null && int.TryParse(s, out result) ? result : def;
        }

        private static int GetDefaultDay()
        {
            return new Random().Next(1, 7);
        }

        public static int GetDefaultTime()
        {
            return new Random().Next(1, 5);
        }

        public static string DateTimeToString(DateTime dateTime)
        {
            // Round-trip format time
            DateTime rtime = DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
            return rtime.ToString("o");
        }

        public static DateTime StringToDateTime(string dateTimeString)
        {
            // Round-trip format time
            DateTime dateTime = DateTime.ParseExact(dateTimeString, "o", CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
            return dateTime;
        }

        public static bool TryParseStringToDateTime(string dateTimeString, out DateTime dateTime)
        {
            // Round-trip format time
            return DateTime.TryParseExact(dateTimeString, "o", CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out dateTime);
        }

        #endregion

        public string GetSecretyInfo(Session session, string secretType)
        {
            string UUID = string.Empty;
            switch (secretType)
            {
                case HealthCheckSettings.UPLOAD_CREDENTIAL_USER_SECRET:
                    UUID = UserNameSecretUuid;
                    break;
                case HealthCheckSettings.UPLOAD_CREDENTIAL_PASSWORD_SECRET:
                    UUID = PasswordSecretUuid;
                    break;
                case HealthCheckSettings.UPLOAD_TOKEN_SECRET:
                    UUID = UploadTokenSecretUuid;
                    break;
                case HealthCheckSettings.DIAGNOSTIC_TOKEN_SECRET:
                    UUID = DiagnosticTokenSecretUuid;
                    break;
                default:
                    log.ErrorFormat("Error getting the {0} from the xapi secret", secretType);
                    break;
            }

            if (session == null || string.IsNullOrEmpty(UUID))
                return null;
            try
            {
                string opaqueref = Secret.get_by_uuid(session, UUID);
                return Secret.get_value(session, opaqueref);
            }
            catch (Exception e)
            {
                log.Error("Exception getting the upload token from the xapi secret", e);
                return null;
            }
        }

        public string GetSecretyInfo(IXenConnection connection, string secretType)
        {
            if (connection == null)
                return null;
            return GetSecretyInfo(connection.Session, secretType);
        }

        public bool TryGetExistingTokens(IXenConnection connection, out string uploadToken, out string diagnosticToken)
        {
            uploadToken = null;
            diagnosticToken = null;
            if (connection == null)
                return false;

            uploadToken = GetSecretyInfo(connection, UPLOAD_TOKEN_SECRET);
            diagnosticToken = GetSecretyInfo(connection, DIAGNOSTIC_TOKEN_SECRET);

            if (!String.IsNullOrEmpty(uploadToken) && !String.IsNullOrEmpty(diagnosticToken))
                return true;

            return TryGetExistingTokensFromOtherConnections(connection, out uploadToken, out diagnosticToken);
        }

        private static bool TryGetExistingTokensFromOtherConnections(IXenConnection currentConnection, out string uploadToken, out string diagnosticToken)
        {
            uploadToken = null;
            diagnosticToken = null; 
            foreach (var connection in ConnectionsManager.XenConnectionsCopy)
            {
                if (connection == currentConnection || !connection.IsConnected)
                    continue;
                var poolOfOne = Helpers.GetPoolOfOne(connection);
                if (poolOfOne != null)
                {
                    var healthCheckSettings = poolOfOne.HealthCheckSettings();
                    uploadToken = healthCheckSettings.GetSecretyInfo(connection, UPLOAD_TOKEN_SECRET);
                    diagnosticToken = healthCheckSettings.GetSecretyInfo(connection, DIAGNOSTIC_TOKEN_SECRET);
                    if (!String.IsNullOrEmpty(uploadToken) && !String.IsNullOrEmpty(diagnosticToken))
                        return true;
                }
            }
            return false;
        }

        internal static DiagnosticAlertSeverity StringToDiagnosticAlertSeverity(string severity)
        {
            DiagnosticAlertSeverity result;
            if (Enum.TryParse(severity, out result))
                return result;
            return DiagnosticAlertSeverity.Info;
        }
    }
}
