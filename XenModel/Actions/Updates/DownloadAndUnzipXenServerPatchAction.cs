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
using System.Net;
using System.ComponentModel;
using System.Threading;
using System.IO;
using XenCenterLib.Archive;

namespace XenAdmin.Actions
{
    internal enum DownloadState { InProgress, Cancelled, Completed, Error };

    public class DownloadAndUnzipXenServerPatchAction : AsyncAction
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private const int SLEEP_TIME_TO_CHECK_DOWNLOAD_STATUS_MS = 900;
        private const int MAX_NUMBER_OF_TRIES = 5; //If you consider increasing this for any reason (I think 5 is already more than enough), have a look at nextSleepMs in DownloadFile() as well.
        private Random random = new Random();

        private readonly Uri address;
        private readonly string zippedFileName;
        private readonly string updateName;
        private readonly string[] updateFileExtensions;
        private readonly bool downloadUpdate;
        private DownloadState patchDownloadState;
        private Exception patchDownloadError;

        public string PatchPath
        {
            get; private set;
        }

        public DownloadAndUnzipXenServerPatchAction(string patchName, Uri uri, string outputFileName, bool suppressHist, params string[] updateFileExtensions)
            : base(null, uri == null ?  string.Format(Messages.UPDATES_WIZARD_EXTRACT_ACTION_TITLE, patchName)
            : string.Format(Messages.DOWNLOAD_AND_EXTRACT_ACTION_TITLE, patchName), string.Empty, suppressHist)
        {
            updateName = patchName;
            address = uri;
            downloadUpdate = address != null;
            zippedFileName = outputFileName;
            this.updateFileExtensions = updateFileExtensions;
        }

        private void DownloadFile()
        {
            int errorCount = 0;
            int nextSleepMs = 0;
            bool needToRetry = true;

            while (errorCount < MAX_NUMBER_OF_TRIES && needToRetry)
            {
                needToRetry = false;

                using (var client = new WebClient())
                {
                    try
                    {
                        client.Proxy = XenAdminConfigManager.Provider.GetProxyFromSettings(null, false);

                        //register download events
                        client.DownloadProgressChanged += client_DownloadProgressChanged;
                        client.DownloadFileCompleted += client_DownloadFileCompleted;
                        //start the download
                        client.DownloadFileAsync(address, zippedFileName);

                        patchDownloadState = DownloadState.InProgress;
                        bool patchDownloadCancelling = false;

                        //wait for the file to be downloaded
                        while (patchDownloadState == DownloadState.InProgress)
                        {
                            if (!patchDownloadCancelling && (Cancelling || Cancelled))
                            {
                                Description = Messages.DOWNLOAD_AND_EXTRACT_ACTION_DOWNLOAD_CANCELLED_DESC;
                                client.CancelAsync();
                                patchDownloadCancelling = true;
                            }
                            Thread.Sleep(SLEEP_TIME_TO_CHECK_DOWNLOAD_STATUS_MS);
                        }

                        if (patchDownloadState == DownloadState.Cancelled)
                            throw new CancelledException();

                        if (patchDownloadState == DownloadState.Error)
                        {
                            needToRetry = true;

                            // this many errors so far - including this one
                            errorCount++;

                            // logging only, it will retry again.
                            log.ErrorFormat("Error while downloading from '{0}'. Number of errors so far (including this): {1}. Trying maximum {2} times.", address, errorCount, MAX_NUMBER_OF_TRIES);
                            log.Error(patchDownloadError ?? new Exception(Messages.ERROR_UNKNOWN));
                            
                            // wait for some randomly increased amount of time after each retry
                            nextSleepMs += random.Next(5000);
                            Thread.Sleep(nextSleepMs);
                        }
                    }
                    finally
                    {
                        //deregister download events
                        client.DownloadProgressChanged -= client_DownloadProgressChanged;
                        client.DownloadFileCompleted -= client_DownloadFileCompleted;
                    }
                }
            }

            //if this is still the case after having retried MAX_RETRY number of times.
            if (patchDownloadState == DownloadState.Error)
            {
                log.ErrorFormat("Giving up - MAX_NUMBER_OF_RETRIES_IF_FAILED has been reached.");
                            
                MarkCompleted(patchDownloadError ?? new Exception(Messages.ERROR_UNKNOWN));
            }

        }

        private void ExtractFile()
        {
            ArchiveIterator iterator = null;
            try
            {
                using (Stream stream = new FileStream(zippedFileName, FileMode.Open, FileAccess.Read))
                {
                    iterator = ArchiveFactory.Reader(ArchiveFactory.Type.Zip, stream);
                    DotNetZipZipIterator zipIterator = iterator as DotNetZipZipIterator;
                    if (zipIterator != null)
                    {
                        zipIterator.CurrentFileExtractProgressChanged +=
                            archiveIterator_CurrentFileExtractProgressChanged;
                    }

                    while (iterator.HasNext())
                    {
                        string currentExtension = Path.GetExtension(iterator.CurrentFileName()).Replace(".","");

                        if (Array.Exists(updateFileExtensions, item => item == currentExtension))
                        {
                            string path = downloadUpdate ? Path.Combine(Path.GetDirectoryName(zippedFileName), iterator.CurrentFileName())
                                : Path.Combine(Path.GetTempPath(), iterator.CurrentFileName());

                            log.InfoFormat("Found '{0}' in the downloaded archive when looking for a '{1}' file. Extracting...", iterator.CurrentFileName(), currentExtension);

                            using (Stream outputStream = new FileStream(path, FileMode.Create))
                            {
                                iterator.ExtractCurrentFile(outputStream);
                                PatchPath = path;

                                log.InfoFormat("Update file extracted to '{0}'", path);
                                
                                break;
                            }
                        }
                    }

                    if (zipIterator != null)
                    {
                        zipIterator.CurrentFileExtractProgressChanged -=
                            archiveIterator_CurrentFileExtractProgressChanged;
                    }
                }
            }
            catch (Exception e)
            {
                log.ErrorFormat("Exception occurred when extracting downloaded archive: {0}", e.Message);
                throw new Exception(Messages.DOWNLOAD_AND_EXTRACT_ACTION_EXTRACTING_ERROR);
            }
            finally
            {
                if (iterator != null)
                    iterator.Dispose();

                if (downloadUpdate)
                    File.Delete(zippedFileName);
            }
            
            if (string.IsNullOrEmpty(PatchPath) && downloadUpdate)
            {
                MarkCompleted(new Exception(Messages.DOWNLOAD_AND_EXTRACT_ACTION_FILE_NOT_FOUND));
                log.InfoFormat("The downloaded archive does not contain a file with any of the following extensions: {0}", string.Join(", ", updateFileExtensions));
            }
        }

        protected override void Run()
        {
            if (downloadUpdate)
            {
                log.InfoFormat("Downloading update '{0}' (from from '{1}') to '{2}'", updateName, address, zippedFileName);


                Description = string.Format(Messages.DOWNLOAD_AND_EXTRACT_ACTION_DOWNLOADING_DESC, updateName);
                LogDescriptionChanges = false;
                DownloadFile();
                LogDescriptionChanges = true;

                if (IsCompleted || Cancelled)
                    return;

                if (Cancelling)
                    throw new CancelledException();
            }

            log.DebugFormat("Extracting XenServer patch '{0}'", updateName);
            Description = string.Format(Messages.DOWNLOAD_AND_EXTRACT_ACTION_EXTRACTING_DESC, updateName);
            ExtractFile();
            log.DebugFormat("Extracting XenServer patch '{0}' completed", updateName);

            Description = Messages.COMPLETED;
            MarkCompleted();
        }

        void archiveIterator_CurrentFileExtractProgressChanged(object sender, ExtractProgressChangedEventArgs e)
        {
            int pc = downloadUpdate ? 95 + (int)(5.0 * e.BytesTransferred / e.TotalBytesToTransfer) : (int)(100.0 * e.BytesTransferred / e.TotalBytesToTransfer);
            if (pc != PercentComplete)
                PercentComplete = pc;
        }

        void client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            int pc = (int)(95.0 * e.BytesReceived / e.TotalBytesToReceive);
            if (pc != PercentComplete)
            {
                DownloadProgressDescription
                    = Description 
                    = string.Format(Messages.DOWNLOAD_AND_EXTRACT_ACTION_DOWNLOADING_DETAILS_DESC, updateName,
                                            Util.DiskSizeString(e.BytesReceived),
                                            Util.DiskSizeString(e.TotalBytesToReceive));
                PercentComplete = pc;
            }
        }

        void client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Cancelled) //user cancelled
            {
                patchDownloadState = DownloadState.Cancelled;
                log.DebugFormat("XenServer patch '{0}' download cancelled by the user", updateName);
                return;
            }

            if (e.Error != null) //failure
            {
                patchDownloadError = e.Error;
                log.DebugFormat("XenServer patch '{0}' download failed", updateName);
                patchDownloadState = DownloadState.Error;
                return;
            }

            //success
            patchDownloadState = DownloadState.Completed;
            log.DebugFormat("XenServer patch '{0}' download completed successfully", updateName);
        }

        public override void RecomputeCanCancel()
        {
            CanCancel = !Cancelling && !IsCompleted && (patchDownloadState == DownloadState.InProgress);
        }

        protected override void CancelRelatedTask()
        {
        }

        public string DownloadProgressDescription { get; set; }
    }
}
