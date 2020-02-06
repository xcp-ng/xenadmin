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
using System.ComponentModel;
using System.Collections.Generic;
using System.Text;
using System.Net;

using XenAdmin.Core;
using System.IO;
using XenAdmin.Network;
using System.Threading;
using XenAPI;


namespace XenAdmin.Actions.Wlb
{
    public class WlbReportAction : AsyncAction
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly string report;
        private readonly string reportName;
        private readonly bool hideException;
        private readonly Dictionary<string, string> parms;

        private Exception exception = null;
        private string result = null;

        public WlbReportAction(IXenConnection connection, 
                                Host host, 
                                string report, 
                                string reportName, 
                                bool hideException, 
                                Dictionary<string, string> parms)
            : base(connection, string.Format(Messages.ACTION_WLB_REPORT, report, host.Name(), Helpers.GetName(connection)),
                   Messages.ACTION_EXPORT_DESCRIPTION_PREPARING, true)
        {
            this.report = report;
            this.reportName = reportName;
            this.hideException = hideException;
            this.parms = parms;

            Pool = Helpers.GetPool(connection);
            Host = host;
        }


        protected override void Run()
        {
            Description = Messages.ACTION_WLB_REPORT_DOWNLOADING;

            RelatedTask = XenAPI.Task.create(Session,
                string.Format(Messages.ACTION_WLB_REPORT_TASK_NAME, reportName),
                string.Format(Messages.ACTION_WLB_REPORT_TASK_DESCRIPTION, reportName));

            UriBuilder uriBuilder = new UriBuilder(Session.Url);
            uriBuilder.Path = "wlb_report";
            uriBuilder.Query = string.Format("session_id={0}&report={1}&task_id={2}",
                Uri.EscapeDataString(Session.opaque_ref),
                Uri.EscapeDataString(report),
                Uri.EscapeDataString(RelatedTask.opaque_ref));

            foreach (string k in parms.Keys)
            {
                string v = parms[k];
                uriBuilder.Query = uriBuilder.Query.Substring(1) + string.Format("&{0}={1}", k, v);
            }

            log.DebugFormat("Downloading report {0}.", report);

            // The DownloadFile call will block, so we need a separate thread to poll for task status.
            Thread taskThread = new Thread((ThreadStart)progressPoll);
            taskThread.Name = "Progress polling thread for WLBReportAction for " + report.Ellipsise(20);
            taskThread.IsBackground = true;
            taskThread.Start();

            try
            {
                HttpGet(uriBuilder.Uri);
            }
            catch (Exception e)
            {
                if (XenAPI.Task.get_status(Session, RelatedTask.opaque_ref) == XenAPI.task_status_type.pending &&
                    XenAPI.Task.get_progress(Session, RelatedTask.opaque_ref) == 0)
                {
                    // If task is pending and has zero progress, it probably hasn't been started,
                    // which probably means there was an exception in the GUI code before the
                    // action got going. Kill the task so that we don't block forever on
                    // taskThread.Join(). Brought to light by CA-11100.
                    DestroyTask();
                }
                if (exception == null)
                    exception = e;
            }

            taskThread.Join();

            if (Cancelling || exception is CancelledException)
            {
                log.InfoFormat("WLB report download for {0} cancelled", report);
                Description = Messages.ACTION_WLB_REPORT_CANCELLED;
                throw new CancelledException();
            }
            else if (exception != null)
            {
                log.Warn(string.Format("WLB report download for {0} failed", report), exception);
                
                // When using this method for acquiring report configuration information
                // we can hide the exception as a second attempt will be made with local
                // config approach
                if (!hideException)
                {
                    if (exception.Message.Contains("HTTP/1.1 403"))
                    {
                        Description = Messages.WLB_REPORT_HTTP_403;
                    }
                    else if (exception.Message.Contains("HTTP/1.0 500"))
                    {
                        Description = Messages.WLB_REPORT_HTTP_500;
                    }
                    else
                    {
                        Description = Messages.ACTION_WLB_REPORT_FAILED;
                    }

                    throw exception;
                }
            }
            else
            {
                log.InfoFormat("WLB report download for {0} successful", report);
                Description = Messages.ACTION_WLB_REPORT_SUCCESSFUL;

                // filter out CDATA
                if (result.Contains("<![CDATA["))
                    result = result.Replace("<![CDATA[", String.Empty).Replace("]]>",String.Empty);

                Result = result;
            }
        }

        private void HttpGet(Uri uri)
        {
            int BUFSIZE = 1024;
            byte[] buf = new byte[BUFSIZE];
            using (MemoryStream ms = new MemoryStream())
            {
                using (Stream http = HTTPHelper.GET(uri, Connection, false))
                {
                    while (true)
                    {
                        int n = http.Read(buf, 0, BUFSIZE);
                        if (n <= 0)
                        {
                            result = new String(Encoding.UTF8.GetChars(ms.ToArray()));
                            return;
                        }
                        ms.Write(buf, 0, n);
                        if (Cancelling)
                            return;
                    }
                }
            }
        }

        private void progressPoll()
        {
            try
            {
                PollToCompletion();
            }
            catch (Exception e)
            {
                if (exception == null)
                    exception = e;
            }
        }
    }
}
