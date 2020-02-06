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
using System.Linq;
using System.Net.NetworkInformation;
using XenCenterLib.Archive;

namespace XenAdmin.Actions
{
    internal enum DownloadState
    {
        InProgress,
        Cancelled,
        Completed,
        Error
    };

    public class DownloadAndUnzipXenServerPatchAction : AsyncAction, IByteProgressAction
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private const int SLEEP_TIME_TO_CHECK_DOWNLOAD_STATUS_MS = 900;
        private const int SLEEP_TIME_BEFORE_RETRY_MS = 5000;

        //If you consider increasing this for any reason (I think 5 is already more than enough), have a look at the usage of SLEEP_TIME_BEFORE_RETRY_MS in DownloadFile() as well.
        private const int MAX_NUMBER_OF_TRIES = 5;

        private readonly Uri address;
        private readonly string outputFileName;
        private readonly string updateName;
        private readonly string[] updateFileSuffixes;
        private readonly bool downloadUpdate;
        private readonly bool skipUnzipping;
        private DownloadState patchDownloadState;
        private Exception patchDownloadError;

        public string PatchPath { get; private set; }

        public string ByteProgressDescription { get; set; }

        public DownloadAndUnzipXenServerPatchAction(string patchName, Uri uri, string outputFileName, bool suppressHist,
            params string[] updateFileExtensions)
            : base(null, uri == null
                ? string.Format(Messages.UPDATES_WIZARD_EXTRACT_ACTION_TITLE, patchName)
                : string.Format(Messages.DOWNLOAD_AND_EXTRACT_ACTION_TITLE, patchName), string.Empty, suppressHist) 
        {
            updateName = patchName;
            address = uri;
            downloadUpdate = address != null;
            updateFileSuffixes = (from item in updateFileExtensions select '.' + item).ToArray();
            skipUnzipping = downloadUpdate && updateFileSuffixes.Any(item => address.ToString().Contains(item));
            this.outputFileName = outputFileName;
        }

        private WebClient client;

        private void DownloadFile()
        {
            int errorCount = 0;
            bool needToRetry = false;

            client = new WebClient();
            //register download events
            client.DownloadProgressChanged += client_DownloadProgressChanged;
            client.DownloadFileCompleted += client_DownloadFileCompleted;

            // register event handler to detect changes in network connectivity
            NetworkChange.NetworkAvailabilityChanged += NetworkAvailabilityChanged;

            try
            {
                do
                {
                    if (needToRetry)
                        Thread.Sleep(SLEEP_TIME_BEFORE_RETRY_MS);

                    needToRetry = false;

                    client.Proxy = XenAdminConfigManager.Provider.GetProxyFromSettings(null, false);

                    //start the download
                    patchDownloadState = DownloadState.InProgress;
                    client.DownloadFileAsync(address, outputFileName);

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
                        log.ErrorFormat(
                            "Error while downloading from '{0}'. Number of errors so far (including this): {1}. Trying maximum {2} times.",
                            address, errorCount, MAX_NUMBER_OF_TRIES);

                        if  (patchDownloadError == null)
                            log.Error("An unknown error occurred.");
                        else
                            log.Error(patchDownloadError);
                    }
                } while (errorCount < MAX_NUMBER_OF_TRIES && needToRetry);
            }
            finally
            {
                //deregister download events
                client.DownloadProgressChanged -= client_DownloadProgressChanged;
                client.DownloadFileCompleted -= client_DownloadFileCompleted;

                NetworkChange.NetworkAvailabilityChanged -= NetworkAvailabilityChanged;

                client.Dispose();
            }

            //if this is still the case after having retried MAX_NUMBER_OF_TRIES number of times.
            if (patchDownloadState == DownloadState.Error)
            {
                log.ErrorFormat("Giving up - Maximum number of retries ({0}) has been reached.", MAX_NUMBER_OF_TRIES);

                MarkCompleted(patchDownloadError ?? new Exception(Messages.ERROR_UNKNOWN));
            }

        }

        private void NetworkAvailabilityChanged(object sender, NetworkAvailabilityEventArgs e)
        {
            if (!e.IsAvailable && client != null && patchDownloadState == DownloadState.InProgress)
            {
                patchDownloadError = new WebException(Messages.NETWORK_CONNECTIVITY_ERROR);
                patchDownloadState = DownloadState.Error;
                client.CancelAsync();
            }
        }

        private void ExtractFile()
        {
            ArchiveIterator iterator = null;
            DotNetZipZipIterator zipIterator =  null;

            try
            {
                using (Stream stream = new FileStream(outputFileName, FileMode.Open, FileAccess.Read))
                {
                    iterator = ArchiveFactory.Reader(ArchiveFactory.Type.Zip, stream);
                    zipIterator = iterator as DotNetZipZipIterator;

                    if (zipIterator != null)
                        zipIterator.CurrentFileExtractProgressChanged += archiveIterator_CurrentFileExtractProgressChanged;

                    while (iterator.HasNext())
                    {
                        string currentExtension = Path.GetExtension(iterator.CurrentFileName());

                        if (updateFileSuffixes.Any(item => item == currentExtension))
                        {
                            string path = downloadUpdate
                                ? Path.Combine(Path.GetDirectoryName(outputFileName), iterator.CurrentFileName())
                                : Path.Combine(Path.GetTempPath(), iterator.CurrentFileName());

                            log.InfoFormat(
                                "Found '{0}' in the downloaded archive when looking for a '{1}' file. Extracting...",
                                iterator.CurrentFileName(), currentExtension);

                            using (Stream outputStream = new FileStream(path, FileMode.Create))
                            {
                                iterator.ExtractCurrentFile(outputStream);
                                PatchPath = path;

                                log.InfoFormat("Update file extracted to '{0}'", path);
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                log.Error("Exception occurred when extracting downloaded archive.", e);
                throw new Exception(Messages.DOWNLOAD_AND_EXTRACT_ACTION_EXTRACTING_ERROR);
            }
            finally
            {
                if (zipIterator != null)
                    zipIterator.CurrentFileExtractProgressChanged -= archiveIterator_CurrentFileExtractProgressChanged;

                if (iterator != null)
                    iterator.Dispose();

                if (downloadUpdate)
                {
                    try { File.Delete(outputFileName); }
                    catch { }
                }
            }

            if (string.IsNullOrEmpty(PatchPath) && downloadUpdate)
            {
                MarkCompleted(new Exception(Messages.DOWNLOAD_AND_EXTRACT_ACTION_FILE_NOT_FOUND));
                log.InfoFormat(
                    "The downloaded archive does not contain a file with any of the following extensions: {0}",
                    string.Join(", ", updateFileSuffixes));
            }
        }

        protected override void Run()
        {
            if (downloadUpdate)
            {
                log.InfoFormat("Downloading update '{0}' (from '{1}') to '{2}'", updateName, address, outputFileName);
                Description = string.Format(Messages.DOWNLOAD_AND_EXTRACT_ACTION_DOWNLOADING_DESC, updateName);
                LogDescriptionChanges = false;
                DownloadFile();
                LogDescriptionChanges = true;

                if (IsCompleted || Cancelled)
                    return;

                if (Cancelling)
                    throw new CancelledException();
            }

            if (skipUnzipping)
            {
                try
                {
                    string newFilePath = Path.Combine(Path.GetDirectoryName(outputFileName), updateName);
                    if (File.Exists(newFilePath))
                        File.Delete(newFilePath);
                    File.Move(outputFileName, newFilePath);
                    PatchPath = newFilePath;
                    log.DebugFormat("XenServer patch '{0}' is ready", updateName);
                }
                catch (Exception e)
                {
                    log.Error("Exception occurred when preparing archive.", e);
                    throw;
                }
            }
            else
            {
                log.DebugFormat("Extracting XenServer patch '{0}'", updateName);
                Description = string.Format(Messages.DOWNLOAD_AND_EXTRACT_ACTION_EXTRACTING_DESC, updateName);
                ExtractFile();
                log.DebugFormat("Extracting XenServer patch '{0}' completed", updateName);   
            }
            
            Description = Messages.COMPLETED;
            MarkCompleted();
        }

        void archiveIterator_CurrentFileExtractProgressChanged(long bytesTransferred, long totalBytesToTransfer)
        {
            int pc = downloadUpdate ? 95 + (int)(5.0 * bytesTransferred / totalBytesToTransfer) : (int)(100.0 * bytesTransferred / totalBytesToTransfer);
            PercentComplete = pc;
        }

        void client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            int pc = (int)(95.0 * e.BytesReceived / e.TotalBytesToReceive);
            var descr = string.Format(Messages.DOWNLOAD_AND_EXTRACT_ACTION_DOWNLOADING_DETAILS_DESC, updateName,
                                            Util.DiskSizeString(e.BytesReceived, "F1"),
                                            Util.DiskSizeString(e.TotalBytesToReceive));
            ByteProgressDescription = descr;
            Tick(pc, descr);
        }

        void client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Cancelled && patchDownloadState == DownloadState.Error) // cancelled due to network connectivity issue (see NetworkAvailabilityChanged)
                return;

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

    }
}
