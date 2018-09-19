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
using System.IO;
using System.Threading;
using XenServerHealthCheck;

namespace XenAdmin.Actions
{
    public class UploadServerStatusReportAction: AsyncAction
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly string uploadToken;
        private readonly string bundleToUpload;
        private readonly string caseNumber;

        private const string UPLOAD_URL = "/feeds/api/";
        private readonly string UPLOAD_DOMAIN_NAME = "https://rttf.citrix.com";

        private CancellationTokenSource cts;

        public UploadServerStatusReportAction(string bundleToUpload, string uploadToken, string caseNumber, bool suppressHistory)
            : base(null, Messages.ACTION_UPLOAD_SERVER_STATUS_REPORT, Messages.ACTION_UPLOAD_SERVER_STATUS_REPORT_PROGRESS, suppressHistory)
        {
            this.bundleToUpload = bundleToUpload;
            this.uploadToken = uploadToken;
            this.caseNumber = caseNumber;
        }

        public UploadServerStatusReportAction(string bundleToUpload, string uploadToken, string caseNumber, string uploadDomainName, bool suppressHistory)
            : this(bundleToUpload, uploadToken, caseNumber, suppressHistory)
        {
            if (!string.IsNullOrEmpty(uploadDomainName))
                UPLOAD_DOMAIN_NAME = uploadDomainName;
        }

        protected override void Run()
        {
            string uploadUuid;
            try
            {
                uploadUuid = BundleUpload();
            }
            catch (Exception e)
            {
                log.Error("Exception during upload", e);
                throw new Exception(Messages.ACTION_UPLOAD_SERVER_STATUS_REPORT_FAILED, e);
            }

            if (Cancelling || Cancelled)
            {
                throw new CancelledException();
            }
              
            else if (string.IsNullOrEmpty(uploadUuid))
            {
                // Fail to upload the zip to CIS server.
                log.ErrorFormat("Fail to upload the Server Status Report {0} to CIS server {1}", bundleToUpload, UPLOAD_DOMAIN_NAME);
                throw new Exception(Messages.ACTION_UPLOAD_SERVER_STATUS_REPORT_FAILED);
            }

            Description = Messages.COMPLETED;
        }

        public string BundleUpload()
        {
            if (string.IsNullOrEmpty(bundleToUpload) || !File.Exists(bundleToUpload))
            {
                log.ErrorFormat("No Server Status Report to upload");
                return "";
            }

            cts = new CancellationTokenSource();

            RecomputeCanCancel();

            // Upload the zip file to CIS uploading server.
            var uploadUrl = string.Format("{0}{1}", UPLOAD_DOMAIN_NAME, UPLOAD_URL);
            XenServerHealthCheckUpload upload = new XenServerHealthCheckUpload(uploadToken, 9, uploadUrl, null);
            string uploadUuid = upload.UploadZip(bundleToUpload, caseNumber, cts.Token);

            // Return the uuid of upload.
            return uploadUuid;
        }

        public override void RecomputeCanCancel()
        {
            CanCancel = (cts != null && cts.Token.CanBeCanceled);
        }

        protected override void CancelRelatedTask()
        {
            Description = Messages.CANCELING;

            if (cts != null)
                cts.Cancel();
        }
    }
}
