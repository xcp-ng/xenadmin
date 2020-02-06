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
using Ionic.Zip;

namespace XenCenterLib.Archive
{
    public class DotNetZipZipIterator : ArchiveIterator
    {
        private ZipFile zipFile;
        private IEnumerator<ZipEntry> enumerator;
        private ZipEntry zipEntry;
        private bool disposed;

        /// <summary>
        /// delegate params: long bytesTransferred, long totalBytesToTransfer
        /// </summary>
        public event Action<long, long> CurrentFileExtractProgressChanged;
        public event Action CurrentFileExtractCompleted;

        public DotNetZipZipIterator(Stream inputStream)
        {
            Initialise(inputStream);
            disposed = false;
        }

        void zipFile_ExtractProgress(object sender, ExtractProgressEventArgs e)
        {
            switch (e.EventType)
            {
                case ZipProgressEventType.Extracting_EntryBytesWritten:
                    {
                        var handler = CurrentFileExtractProgressChanged;
                        handler?.Invoke(e.BytesTransferred, e.TotalBytesToTransfer);
                    }
                    break;
                case ZipProgressEventType.Extracting_AfterExtractEntry:
                    {
                        var handler = CurrentFileExtractCompleted;
                        handler?.Invoke();
                    }
                    break;
            }
        }

        private void Initialise(Stream zipStream)
        {
            try
            {
                zipFile = ZipFile.Read(zipStream);
            }
            catch (ZipException e)
            {
                throw new ArgumentException("Cannot read input as a ZipFile", "zipStream", e);
            }
            
            enumerator = zipFile.GetEnumerator();
            zipFile.ExtractProgress += zipFile_ExtractProgress;
        }

        public override void SetBaseStream(Stream inputStream)
        {
            Initialise(inputStream);
            disposed = false;
        }

        ~DotNetZipZipIterator()
        {
            Dispose();
        }

        public override bool HasNext()
        {
            if (enumerator != null && enumerator.MoveNext())
            {
                zipEntry = enumerator.Current;
                return true;
            }
            return false;
        }

        public override string CurrentFileName()
        {
            if (zipEntry == null)
                return String.Empty;

            return zipEntry.FileName;
        }

        public override long CurrentFileSize()
        {
            if (zipEntry == null)
                return 0;

            return zipEntry.UncompressedSize;
        }

        public override DateTime CurrentFileModificationTime()
        {
            if (zipEntry == null)
                return new DateTime();

            return zipEntry.LastModified;
        }

        public override bool IsDirectory()
        {
            if (zipEntry == null)
                return false;

            return zipEntry.IsDirectory;
        }

        public override void ExtractCurrentFile(Stream extractedFileContents)
        {
            if (IsDirectory())
                return;

            zipEntry.Extract(extractedFileContents);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if(disposing)
            {
                if(!disposed)
                {
                    if (zipFile != null)
                    {
                        zipFile.ExtractProgress -= zipFile_ExtractProgress;
                        zipFile.Dispose();
                    }

                    disposed = true;
                }               
            }
        }
    }
}
