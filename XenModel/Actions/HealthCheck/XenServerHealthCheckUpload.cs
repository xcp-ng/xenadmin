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
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Script.Serialization;
using XenAdmin;
using XenAdmin.Network;
using XenAdmin.Core;

namespace XenServerHealthCheck
{
    public class XenServerHealthCheckUpload : IDisposable
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly string uploadUrl = "https://rttf.citrix.com/feeds/api/";

        private const string UPLOAD_CHUNK_STRING = "{0}upload_raw_chunk/?id={1}&offset={2}";
        private const string INITIATE_UPLOAD_STRING = "{0}bundle/?size={1}&name={2}";
        private const int CHUNK_SIZE = 1 * 1024 * 1024;
        private const int CHUNK_UPLOAD_TRIES = 3;
        private JavaScriptSerializer serializer;
        private HttpClient httpClient;

        private readonly int verbosityLevel;
        private readonly string uploadToken;

        /// <summary>
        /// Fired when progress changes; the first param is bytes uploaded, the second total bytes
        /// </summary>
        public event Action<long, long> ProgressChanged;

        public XenServerHealthCheckUpload(string token, int verbosity, string uploadUrl, IXenConnection connection)
        {
            uploadToken = token;
            verbosityLevel = verbosity;
            serializer = new JavaScriptSerializer();
            serializer.MaxJsonLength = int.MaxValue;
            if (!string.IsNullOrEmpty(uploadUrl))
            {
                this.uploadUrl = uploadUrl;
            }

            var proxy = XenAdminConfigManager.Provider.GetProxyFromSettings(connection, false);
            HttpClientHandler handler = new HttpClientHandler()
            {
                Proxy = proxy,
                UseProxy = proxy != null
            };
           
            httpClient = new HttpClient(handler, true);
            httpClient.DefaultRequestHeaders.Add("Authorization", "BT " + uploadToken);
        }

        /// <summary>
        /// Request an upload and fetch the upload id from CIS
        /// </summary>
        private string InitiateUpload(string fileName, long size, string caseNumber, System.Threading.CancellationToken cancel)
        {
            // Request a new bundle upload to CIS server.
            string url = string.Format(INITIATE_UPLOAD_STRING, uploadUrl, size, fileName.UrlEncode());
            if (!string.IsNullOrEmpty(caseNumber))
                url += "&sr=" + caseNumber;
            log.InfoFormat("InitiateUpload, UPLOAD_URL: {0}", url);
            
            // Add the log verbosity level in json content.
            string verbosity = "{\"verbosity\":\""+ verbosityLevel + "\"}";
            
            try
            {
                using (var httpContent = new StringContent(verbosity))
                {
                    httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    using (var response = httpClient.PostAsync(new Uri(url), httpContent, cancel).Result)
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            var respString = response.Content.ReadAsStringAsync().Result;

                            // Get the upload uuid
                            var res = (Dictionary<string, object>) serializer.DeserializeObject(respString);
                            return res.ContainsKey("id") ? (string) res["id"] : "";
                        }
                        log.ErrorFormat("Failed to initiate a CIS upload. POST request returned {0} ({1})", response.StatusCode, response.ReasonPhrase);
                    }
                }
            }
            catch (Exception e)
            {
                log.Error("Exception while initiating a new CIS upload: ", e);
            }

            // Fail to initialize the upload request
            return "";
        }

        private bool UploadChunk(string address, FileStream source, long offset, long thisChunkSize, System.Threading.CancellationToken cancel)
        {
            log.InfoFormat("UploadChunk, UPLOAD_URL: {0}", address);

            source.Position = offset;
            int bytes = Convert.ToInt32(thisChunkSize);
            byte[] buffer = new byte[CHUNK_SIZE];
            int read;
            int totalBytesRead = 0;

            while (bytes > 0 && (read = source.Read(buffer, 0, Math.Min(buffer.Length, bytes))) > 0)
            {
                bytes -= read;
                totalBytesRead += read;
            }

            try
            {
                using (var httpContent = new ByteArrayContent(buffer, 0, totalBytesRead))
                {
                    httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                    using (var response = httpClient.PostAsync(new Uri(address), httpContent, cancel).Result)
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            var respString = response.Content.ReadAsStringAsync().Result;

                            // After sending every chunk upload request to server, response will contain a status indicating if it is complete.
                            var res = (Dictionary<string, object>) serializer.DeserializeObject(respString);
                            log.InfoFormat("The status of chunk upload: {0}", res.ContainsKey("status") ? res["status"] : "");
                            return true;
                        }
                        log.ErrorFormat("Failed to upload the chunk. POST request returned {0} ({1})", response.StatusCode, response.ReasonPhrase);
                    }
                }
               
            }
            catch (Exception e)
            {
                log.Error("Failed to upload the chunk. The exception was: ", e);
            }

            return false;
        }

        /// <summary>
        /// Initiate the upload and then upload the zip file in chunks
        /// </summary>
        public string UploadZip(string fileName, string caseNumber, System.Threading.CancellationToken cancel)
        {
            log.InfoFormat("Initiate the upload for {0}", fileName);
            FileInfo fileInfo = new FileInfo(fileName);
            long size = fileInfo.Length;

            // Fetch the upload UUID from CIS server.
            string uploadUuid = InitiateUpload(Path.GetFileName(fileName), size, caseNumber, cancel);
            if (string.IsNullOrEmpty(uploadUuid))
            {
                log.ErrorFormat("Could not fetch the upload UUID from the CIS server");
                return "";
            }

            // Start to upload zip file.
            log.InfoFormat("Upload server returned Upload UUID: {0}", uploadUuid);
            log.InfoFormat("Start to upload bundle {0}", fileName);

            using (var source = fileInfo.Open(FileMode.Open, FileAccess.Read, FileShare.None))
            {
                long offset = 0;
                while (offset < size)
                {
                    var url = string.Format(UPLOAD_CHUNK_STRING, uploadUrl, uploadUuid, offset);
                    long remainingSize = size - offset;
                    long thisChunkSize = remainingSize > CHUNK_SIZE ? CHUNK_SIZE : remainingSize;

                    try
                    {
                        for (int i = 1; i <= CHUNK_UPLOAD_TRIES; i++)
                        {
                            if (cancel.IsCancellationRequested)
                            {
                                log.Info("Upload cancelled");
                                return "";
                            }

                            if (UploadChunk(url, source, offset, thisChunkSize, cancel))
                            {
                                // This chunk is successfully uploaded
                                offset += thisChunkSize;
                                OnProgressChanged(offset, size);
                                break;
                            }

                            // Fail to upload the chunk for 3 times so fail the bundle upload.
                            if (i == CHUNK_UPLOAD_TRIES)
                            {
                                log.ErrorFormat("Failed to upload the chunk. Tried {0} times", CHUNK_UPLOAD_TRIES);
                                return "";
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        log.Error(e, e);
                        return "";
                    }

                }
            }

            log.InfoFormat("Succeeded to upload bundle {0}", fileName);
            return uploadUuid;
        }

        private void OnProgressChanged(long bytesUploaded, long totalBytes)
        {
            if (ProgressChanged != null)
                ProgressChanged(bytesUploaded, totalBytes);
        }

        #region IDisposable Members
        public void Dispose()
        {
            httpClient.Dispose();
        }
        #endregion
    }
}
