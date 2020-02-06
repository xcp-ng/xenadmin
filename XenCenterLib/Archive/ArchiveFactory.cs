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
using XenCenterLib.Compression;

namespace XenCenterLib.Archive
{
    /// <summary>
    /// A static factory to create an object that will allow the archiving of data
    /// </summary>
    public static class ArchiveFactory
    {
        /// <summary>
        /// Supported types of archive
        /// </summary>
        public enum Type
        {
            Tar,
            TarGz,
            TarBz2,
            Zip
        }


        /// <summary>
        /// Instantiate a class that can read a archive type
        /// </summary>
        /// <param name="archiveType">Type of archive to read</param>
        /// <param name="packagedData">The contents of packaged data</param>
        /// <exception cref="NotSupportedException">if there is not a iterator for a specified archive type</exception>
        /// <returns>ArchiveIterator to allow an archive to be traversed</returns>
        public static ArchiveIterator Reader(Type archiveType, Stream packagedData)
        {
            if (archiveType == Type.Tar)
                return new SharpZipTarArchiveIterator(packagedData);
            if (archiveType == Type.TarGz)
                return new SharpZipTarArchiveIterator(packagedData, CompressionFactory.Type.Gz);
            if (archiveType == Type.TarBz2)
                return new SharpZipTarArchiveIterator(packagedData, CompressionFactory.Type.Bz2);
            if (archiveType == Type.Zip)
                return new DotNetZipZipIterator(packagedData);

            throw new NotSupportedException($"Type {archiveType} is not supported by ArchiveIterator");
        }

        /// <summary>
        /// Instantiate a class that can write a archive type
        /// </summary>
        /// <param name="archiveType">Type of archive to write</param>
        /// <param name="targetPackage">The placed where the packaged data will be stored</param>
        /// <exception cref="NotSupportedException">if there is not a writer for a specified archive type</exception>
        /// <returns>ArchiveWriter to allow an archive to be written</returns>
        public static ArchiveWriter Writer(Type archiveType, Stream targetPackage)
        {
            if (archiveType == Type.Tar)
                return new SharpZipTarArchiveWriter(targetPackage);
            if (archiveType == Type.Zip)
                return new DotNetZipZipWriter(targetPackage);

            throw new NotSupportedException($"Type {archiveType} is not supported by ArchiveWriter");
        }
    }
}
