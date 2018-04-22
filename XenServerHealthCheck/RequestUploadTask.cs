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
using XenAdmin.Network;
using XenAPI;
using XenAdmin.Model;

namespace XenServerHealthCheck
{
    public class RequestUploadTask
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static T Get<T>(Dictionary<string, T> d, string k) where T : class
        {
            return d != null && d.ContainsKey(k) ? d[k] : null;
        }

        private static int IntKey(Dictionary<string, string> d, string k, int def)
        {
            int result;
            string s = Get(d, k);
            return s != null && int.TryParse(s, out result) ? result : def;
        }

        private static bool BoolKey(Dictionary<string, string> d, string k)
        {
            string v = Get(d, k);
            return v == null ? false : v == "true";
        }

        private static double DueAfterHour = 24;
        private static bool CanLock(string UploadLock, bool onDemand)
        {
            if (string.IsNullOrEmpty(UploadLock))
                return true;

            List<string> currentLock = new List<string>(UploadLock.Split('|'));
            DateTime currentTime = DateTime.UtcNow;
            DateTime lockTime = DateTime.Parse(currentLock[1]);

            if (currentLock[0] == Properties.Settings.Default.UUID && onDemand)
                return true;
            
            if (currentLock[0] == Properties.Settings.Default.UUID)
            {
                if ((DateTime.Compare(lockTime.AddHours(DueAfterHour), currentTime) <= 0))
                    return true;
            }
            else if ((DateTime.Compare(lockTime.AddHours(DueAfterHour), currentTime) <= 0))
                return true;
            return false;
        }

        private static int SleepForLockConfirm = 10 * 1000; // 10 seconds
        private static bool getLock(IXenConnection connection, Session session)
        {
            Dictionary<string, string> config = Pool.get_health_check_config(session, connection.Cache.Pools[0].opaque_ref);
            string newUploadLock = Properties.Settings.Default.UUID;
            newUploadLock += "|" + HealthCheckSettings.DateTimeToString(DateTime.UtcNow);
            config[HealthCheckSettings.UPLOAD_LOCK] = newUploadLock;
            Pool.set_health_check_config(session, connection.Cache.Pools[0].opaque_ref, config);
            System.Threading.Thread.Sleep(SleepForLockConfirm);
            config = Pool.get_health_check_config(session, connection.Cache.Pools[0].opaque_ref);
            return config[HealthCheckSettings.UPLOAD_LOCK] == newUploadLock;
        }

        public static bool Request(IXenConnection connection, Session session)
        {
            bool needRetry = false;
            Dictionary<string, string> config = Pool.get_health_check_config(session, connection.Cache.Pools[0].opaque_ref);
            if (BoolKey(config, HealthCheckSettings.STATUS) == false)
            {
                ServerListHelper.instance.removeServerCredential(connection.Hostname);
                log.InfoFormat("Will not report for XenServer {0} that was not Enroll", connection.Hostname);
                return false;
            }
            //Check if there already some service doing the uploading already
            if (CanLock(Get(config, HealthCheckSettings.UPLOAD_LOCK), false) == false)
            {
                log.InfoFormat("Will not report for XenServer {0} that already locked", connection.Hostname);
                return false;
            }

            //No Lock has been set before, Check upload is due
            int intervalInDays = IntKey(config, HealthCheckSettings.INTERVAL_IN_DAYS, HealthCheckSettings.DEFAULT_INTERVAL_IN_DAYS);
            DateTime lastSuccessfulUpload = DateTime.UtcNow;
            bool haveSuccessfulUpload = false;
            if (config.ContainsKey(HealthCheckSettings.LAST_SUCCESSFUL_UPLOAD))
            {
                try
                {

                    lastSuccessfulUpload = HealthCheckSettings.StringToDateTime(Get(config, HealthCheckSettings.LAST_SUCCESSFUL_UPLOAD));
                    haveSuccessfulUpload = true;
                }
                catch (Exception exn)
                {
                    log.Error("Catch exception when Parse LAST_SUCCESSFUL_UPLOAD", exn);
                }
            }

            if (haveSuccessfulUpload)
            {//If last successful update not due. return
                if (DateTime.Compare(DateTime.UtcNow, lastSuccessfulUpload.AddDays(intervalInDays)) < 0)
                {
                    log.InfoFormat("Will not report for XenServer {0} that was not due {1} days", connection.Hostname, intervalInDays);
                    return false;
                }
            }

            if (config.ContainsKey(HealthCheckSettings.LAST_FAILED_UPLOAD))
            {
                try
                {
                    DateTime LastFailedUpload = HealthCheckSettings.StringToDateTime(Get(config, HealthCheckSettings.LAST_FAILED_UPLOAD));

                    int retryInterval = IntKey(config, HealthCheckSettings.RETRY_INTERVAL, HealthCheckSettings.DEFAULT_RETRY_INTERVAL);
                    if (DateTime.Compare(LastFailedUpload.AddDays(retryInterval), DateTime.UtcNow) <= 0)
                    {
                        if ((!haveSuccessfulUpload) ||
                            (DateTime.Compare(lastSuccessfulUpload, LastFailedUpload) < 0))
                        {
                            log.InfoFormat("Retry since retryInterval{0} - {1} > {2} meeted", LastFailedUpload, DateTime.UtcNow, retryInterval);
                            needRetry = true;
                        }
                    }
                }
                catch (Exception exn)
                {
                    log.Error("Catch exception when check if retry was needed", exn);
                }
            }

            DateTime currentTime = DateTime.Now;
            if (!needRetry)
            {//Check if uploading schedule meet only for new upload
                
                DayOfWeek dayOfWeek;
                if (!Enum.TryParse(Get(config, HealthCheckSettings.DAY_OF_WEEK), out dayOfWeek))
                {
                    log.Error("DAY_OF_WEEK not existed");
                    return false;
                }
                if (!config.ContainsKey(HealthCheckSettings.TIME_OF_DAY))
                {
                    log.Error("TIME_OF_DAY not existed");
                    return false;
                }

                int TimeOfDay = IntKey(config, HealthCheckSettings.TIME_OF_DAY, HealthCheckSettings.GetDefaultTime());
                if (currentTime.DayOfWeek != dayOfWeek || currentTime.Hour != TimeOfDay)
                {
                    log.InfoFormat("Will not report for XenServer {0} for incorrect schedule", connection.Hostname);
                    return false;
                }
                log.InfoFormat("Upload schedule for {0} is {1}:{2}, meet current time {3}", connection.Hostname, dayOfWeek, TimeOfDay, currentTime.ToString());
            }
            return getLock(connection, session);
        }

        public static bool OnDemandRequest(IXenConnection connection, Session session)
        {
            Dictionary<string, string> config = Pool.get_health_check_config(session, connection.Cache.Pools[0].opaque_ref);
            if (BoolKey(config, HealthCheckSettings.STATUS) == false)
            {
                log.InfoFormat("Will not report on demand for XenServer {0} that was not Enroll", connection.Hostname);
                return false;
            }

            //Check if there already some service doing the uploading already
            if (CanLock(Get(config, HealthCheckSettings.UPLOAD_LOCK), true) == false)
            {
                log.InfoFormat("Will not report for XenServer {0} that already locked", connection.Hostname);
                return false;
            }

            var newUploadRequest = Get(config, HealthCheckSettings.NEW_UPLOAD_REQUEST);
            if (!string.IsNullOrEmpty(newUploadRequest))
            {
                DateTime newUploadRequestTime;
                try
                {
                    newUploadRequestTime = HealthCheckSettings.StringToDateTime(newUploadRequest);
                }
                catch (Exception exn)
                {
                    log.Error("Exception while parsing NEW_UPLOAD_REQUEST", exn);
                    return false;
                }
                DateTime newUploadRequestDueTime = newUploadRequestTime.AddMinutes(HealthCheckSettings.UPLOAD_REQUEST_VALIDITY_INTERVAL);
                if (DateTime.Compare(newUploadRequestDueTime,  DateTime.UtcNow) >= 1)
                {
                    log.InfoFormat("Will report on demand for XenServer {0} since the demand was requested on {1} (UTC time)", connection.Hostname, newUploadRequestTime);
                    return getLock(connection, session);
                }
                else
                {
                    log.InfoFormat("Will not report on demand for XenServer {0} since the demand requested on {1} (UTC time) expired after {2} minutes",
                        connection.Hostname, newUploadRequestTime, HealthCheckSettings.UPLOAD_REQUEST_VALIDITY_INTERVAL);
                    return false;
                }
            }
            else
                return false;
        }
    }
}
