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
using System.Text;
using ICSharpCode.SharpZipLib.Tar;

namespace XenCenterLib.Archive
{

    public class SharpZipTarArchiveWriter : ArchiveWriter
    {
        private TarOutputStream tar = null;
        private const long bufferSize = 32*1024;
        protected bool disposed;

        public SharpZipTarArchiveWriter(Stream outputStream)
        {
            tar = new TarOutputStream(outputStream);
            disposed = false;
        }

        public override void SetBaseStream(Stream outputStream)
        {
            tar = new TarOutputStream(outputStream);
            disposed = false;
        }

        public override void AddDirectory(string directoryName, DateTime modificationTime)
        {
            StringBuilder sb = new StringBuilder(directoryName);
            
            //Need to add a terminal front-slash to add a directory
            if (!directoryName.EndsWith("/"))
                sb.Append("/");
            TarEntry entry = TarEntry.CreateTarEntry(sb.ToString());
            entry.ModTime = modificationTime;

            tar.PutNextEntry(entry);
            tar.CloseEntry();
        }

        public override void Add(Stream filetoAdd, string fileName, DateTime modificationTime)
        {
            TarEntry entry = TarEntry.CreateTarEntry(fileName);
            entry.Size = filetoAdd.Length;
            entry.ModTime = modificationTime;

            tar.PutNextEntry( entry );
            byte[] buffer = new byte[bufferSize];
            int n;

            //You have to do this because if using a memory stream the pointer will be at the end it
            //it's just been read and this will cause nothing to be written out
            filetoAdd.Position = 0;

            while ((n = filetoAdd.Read(buffer, 0, buffer.Length)) > 0)
            {
                tar.Write(buffer, 0, n);
            }
            
            tar.Flush();
            tar.CloseEntry();
        }

        protected override void Dispose(bool disposing)
        {
            
            if( !disposed )
            {
                if( disposing )
                {
                   if (tar != null)
                    {
                        tar.Dispose();
                    }
                    disposed = true;
                }  
            }
            base.Dispose(disposing);   
        }
    }


}
