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
using ICSharpCode.SharpZipLib.Tar;
using XenCenterLib.Compression;

namespace XenCenterLib.Archive
{

    public class SharpZipTarArchiveIterator : ArchiveIterator
    {
        private TarInputStream tarStream;
        private CompressionStream compressionStream;
        private TarEntry tarEntry;
        private bool disposed;

        public SharpZipTarArchiveIterator(Stream compressedTarFile, CompressionFactory.Type compressionType)
        {
            if (compressionType == CompressionFactory.Type.Gz)
                compressionStream = CompressionFactory.Reader(CompressionFactory.Type.Gz, compressedTarFile);
            else if (compressionType == CompressionFactory.Type.Bz2)
                compressionStream = CompressionFactory.Reader(CompressionFactory.Type.Bz2, compressedTarFile);
            else
                throw new NotSupportedException($"Type {compressionType} is not supported by ArchiveIterator");

            tarStream = new TarInputStream(compressionStream);
            disposed = false;
        }

        public SharpZipTarArchiveIterator(Stream tarFile)
        {
            tarStream = new TarInputStream(tarFile);
            disposed = false;
        }

        public override void SetBaseStream(Stream stream)
        {
            tarStream = new TarInputStream(stream);
            disposed = false;
        }

        ~SharpZipTarArchiveIterator()
        {
            Dispose();
        }

        public override bool HasNext()
        {
            tarEntry = tarStream.GetNextEntry();

            if (tarEntry == null)
                return false;

            return true;
        }

        public override string CurrentFileName()
        {
            if (tarEntry == null)
                return String.Empty;

            return tarEntry.Name;
        }

        public override long CurrentFileSize()
        {
            if (tarEntry == null)
                return 0;

            return tarEntry.Size;
        }

        public override DateTime CurrentFileModificationTime()
        {
            if (tarEntry == null)
                return new DateTime();

            return tarEntry.ModTime;
        }

        public override bool IsDirectory()
        {
            if (tarEntry == null)
                return false;

            return tarEntry.IsDirectory;
        }

        public override void ExtractCurrentFile(Stream extractedFileContents)
        {
            if (IsDirectory())
                return;

            tarStream.CopyEntryContents(extractedFileContents);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if(disposing)
            {
                if(!disposed)
                {
                    tarStream?.Dispose();
                    compressionStream?.Dispose();
                    disposed = true;
                }
            }
        }

        public override bool VerifyCurrentFileAgainstDigest(string algorithmName, byte[] digest)
        {
            return StreamUtilities.VerifyAgainstDigest(tarStream, CurrentFileSize(), algorithmName, digest);
        }
    }
}
