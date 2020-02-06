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
using System.IO;
using System.Net;
using System.Web.Script.Serialization;
using XenAdmin.Model;
using XenAPI;
using System.Linq;

namespace XenAdmin.Actions
{
    public class GetHealthCheckAnalysisResultAction: AsyncAction
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private const string DIAG_RESULT_URL = "/diag_sdk/diag_results/";
        private const string ANALYSIS_PROGRESS_URL = "/diag_sdk/analysis_progress/";
        private readonly string diagnosticDomainName = "https://cis.citrix.com";
       
        public GetHealthCheckAnalysisResultAction(Pool pool, bool suppressHistory)
            : base(pool.Connection, Messages.ACTION_GET_HEALTH_CHECK_RESULT, Messages.ACTION_GET_HEALTH_CHECK_RESULT_PROGRESS, suppressHistory)
        {
            #region RBAC Dependencies
            ApiMethodsToRoleCheck.Add("secret.get_by_uuid");
            ApiMethodsToRoleCheck.Add("secret.get_value");
            ApiMethodsToRoleCheck.Add("pool.set_health_check_config");
            #endregion
        }

        public GetHealthCheckAnalysisResultAction(Pool pool, string diagnosticDomainName, bool suppressHistory)
            : this(pool, suppressHistory)
        {
            if (!string.IsNullOrEmpty(diagnosticDomainName))
                this.diagnosticDomainName = diagnosticDomainName;
        }

        protected override void Run()
        {
            try
            {
                var healthCheckSettings = Pool.HealthCheckSettings();

                var diagnosticToken = healthCheckSettings.GetSecretyInfo(Session, HealthCheckSettings.DIAGNOSTIC_TOKEN_SECRET);
                if (string.IsNullOrEmpty(diagnosticToken))
                {
                    log.DebugFormat("Cannot get the diagnostic result for {0}, because couldn't retrieve the diagnostic token", Pool.Name());
                    Description = Messages.ACTION_GET_HEALTH_CHECK_RESULT_FAILED;
                    return;
                }
                if (!healthCheckSettings.HasUpload)
                {
                    log.DebugFormat("Cannot get the diagnostic result for {0}, because the there is no upload completed yet", Pool.Name());
                    Description = Messages.ACTION_GET_HEALTH_CHECK_RESULT_FAILED;
                    return;
                }


                var analysisProgress = GetAnalysisProgress(diagnosticToken, healthCheckSettings.UploadUuid);

                if (analysisProgress >= 0 && analysisProgress < 100) // check if the progress is a valid value less than 100
                {
                    log.DebugFormat("Analysis for {0} not completed yet", Pool.Name());
                    Description = Messages.COMPLETED;
                    return;
                }

                var analysisResult = GetAnalysisResult(diagnosticToken, healthCheckSettings.UploadUuid);

                if (analysisResult.Count == 0 && analysisProgress == -1 && DateTime.Compare(healthCheckSettings.LastSuccessfulUploadTime.AddMinutes(10), DateTime.UtcNow) > 0)
                {
                    log.DebugFormat("Diagnostic result for {0} is empty. Maybe analysis result is not yet available", Pool.Name());
                    Description = Messages.COMPLETED;
                    return;
                }

                log.Debug("Saving analysis result");
                Dictionary<string, string> newConfig = Pool.health_check_config;
                newConfig[HealthCheckSettings.REPORT_ANALYSIS_SEVERITY] = GetMaxSeverity(analysisResult).ToString();
                newConfig[HealthCheckSettings.REPORT_ANALYSIS_ISSUES_DETECTED] = GetDistinctIssueCount(analysisResult).ToString();
                newConfig[HealthCheckSettings.REPORT_ANALYSIS_UPLOAD_UUID] = healthCheckSettings.UploadUuid;
                newConfig[HealthCheckSettings.REPORT_ANALYSIS_UPLOAD_TIME] = healthCheckSettings.LastSuccessfulUpload;
                Pool.set_health_check_config(Session, Pool.opaque_ref, newConfig);
            }
            catch (Exception e)
            {
                log.Error($"Exception while getting diagnostic result from {diagnosticDomainName}.", e);
                Description = Messages.ACTION_GET_HEALTH_CHECK_RESULT_FAILED;
                throw;
            }
            
            Description = Messages.COMPLETED;
        }

        private DiagnosticAlertSeverity GetMaxSeverity(List<AnalysisResult> diagnosticAlerts)
        {
            var maxSeverity = DiagnosticAlertSeverity.Info;
            foreach (var diagnosticAlert in diagnosticAlerts)
            {
                var severity = HealthCheckSettings.StringToDiagnosticAlertSeverity(diagnosticAlert.severity);
                if (severity > maxSeverity)
                    maxSeverity = severity;
            }
            return maxSeverity;
        }

        private int GetDistinctIssueCount(List<AnalysisResult> issues)
        {
            // get the number of distinct issues (by name)
            return issues.Select(issue => issue.name).Distinct().Count();
        }

        /// <summary>
        /// Tries to retrieve the analysis progress for a particulat upload
        /// </summary>
        /// <param name="diagnosticToken"></param>
        /// <param name="uploadUuid"></param>
        /// <returns>the analysis progress as pecentage, or -1 if the progress cannot be retrieved</returns>
        private double GetAnalysisProgress(string diagnosticToken, string uploadUuid)
        {
            try
            {
                var urlString = string.Format("{0}{1}?upload_id={2}", diagnosticDomainName, ANALYSIS_PROGRESS_URL, uploadUuid);
                var authorizationHeader = "BT " + diagnosticToken;
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(urlString);
                httpWebRequest.Headers.Add("Authorization", authorizationHeader);
                httpWebRequest.Method = "GET";
                httpWebRequest.Proxy = XenAdminConfigManager.Provider.GetProxyFromSettings(null, false);

                string result;
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    result = streamReader.ReadToEnd();
                }

                return ParseAnalysisProgress(result, uploadUuid);
            }
            catch (Exception e)
            {
                log.Debug($"Exception while getting analysis progress result from {diagnosticDomainName}.", e);
                return -1;
            }
        }

        /// <summary>
        /// Deserialize the analysis progress from a JSON object that has "upload_id" as key and a number (percentage from 0 to 100) as a value
        /// </summary>
        /// <param name="jsonString"></param>
        /// <param name="uploadUuid"></param>
        /// <returns>the analysis progress as percentage, or -1 if the JSON object is invalid</returns>
        public static double ParseAnalysisProgress(string jsonString, string uploadUuid)
        {
            if (string.IsNullOrEmpty(jsonString))
                return -1;
            try
            {
                var serializer = new JavaScriptSerializer();
                Dictionary<string, object> result = serializer.DeserializeObject(jsonString) as Dictionary<string, object>;
                var progress = result != null && result.ContainsKey(uploadUuid) ? Convert.ToDouble(result[uploadUuid]) : -1;
                return progress >= 0 && progress <= 100 ? progress : -1;
            }
            catch (Exception e)
            {
                log.Debug($"Exception while deserializing json: {jsonString}.", e);
                return -1;
            }
        }

        private List<AnalysisResult> GetAnalysisResult(string diagnosticToken, string uploadUuid)
        {
            var urlString = string.Format("{0}{1}?upload_uuid={2}", diagnosticDomainName, DIAG_RESULT_URL, uploadUuid);
            var authorizationHeader = "BT " + diagnosticToken;
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(urlString);
            httpWebRequest.Headers.Add("Authorization", authorizationHeader);
            httpWebRequest.Method = "GET";
            httpWebRequest.Proxy = XenAdminConfigManager.Provider.GetProxyFromSettings(null, false);

            string result;
            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                result = streamReader.ReadToEnd();
            }
            TemplateResponse response = new JavaScriptSerializer().Deserialize<TemplateResponse>(result);
            return response.results;
        }

        private class TemplateResponse
        {
            public List<AnalysisResult> results { get; set; }
        }

        private class AnalysisResult
        {
            public string severity { get; set; }
            public string extra_information { get; set; }
            public string owner { get; set; }
            public string crm_id { get; set; }
            public string id { get; set; }
            public List<string> category { get; set; }
            public string analysis_id { get; set; }
            public string priority { get; set; }
            public string product { get; set; }
            public string description { get; set; }
            public DateTime analysis_completion_time { get; set; }
            public string component { get; set; }
            public string name { get; set; }
            public string upload_description { get; set; }
            public string upload_id { get; set; }
            public string recommendations { get; set; }
            public string env_identifier { get; set; }
        }
    }
}
