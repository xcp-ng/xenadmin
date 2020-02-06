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

namespace XenCenterLib.Archive
{
    public abstract class ArchiveWriter : IDisposable
    {
        public abstract void Add(Stream filetoAdd, string fileName, DateTime modificationTime);

        public virtual void SetBaseStream(Stream outputStream)
        {
            throw new NotImplementedException();
        }

        public abstract void AddDirectory(string directoryName, DateTime modificationTime);

        /// <summary>
        /// Disposal hook
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing){ }

        public void CreateArchive(string pathToArchive, Action cancellingDelegate = null, Action<int> progressDelegate = null)
        {
            if (!Directory.Exists(pathToArchive))
                throw new FileNotFoundException("The path " + pathToArchive + " does not exist");

            var files = Directory.GetFiles(pathToArchive, "*.*", SearchOption.AllDirectories);
            for (var i = 0; i < files.Length; i++)
            {
                string filePath = files[i];
                cancellingDelegate?.Invoke();

                using (FileStream fs = File.OpenRead(filePath))
                {
                    Add(fs, CleanRelativePathName(pathToArchive, filePath), File.GetCreationTime(filePath));
                    progressDelegate?.Invoke((int)50.0 * i / files.Length);
                }
            }

            var directories = Directory.GetDirectories(pathToArchive, "*.*", SearchOption.AllDirectories);
            for (var j = 0; j < directories.Length; j++)
            {
                string dirPath = directories[j];
                cancellingDelegate?.Invoke();
                AddDirectory(CleanRelativePathName(pathToArchive, dirPath), Directory.GetCreationTime(dirPath));
                progressDelegate?.Invoke(50 + (int)50.0 * j / directories.Length);
            }
        }

        public void Add(Stream filetoAdd, string fileName)
        {
            Add( filetoAdd, fileName, DateTime.Now );
        }

        public void AddDirectory(string directoryName)
        {
            AddDirectory(directoryName, DateTime.Now);
        }
           
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);   
        }

        private string CleanRelativePathName(string rootPath, string pathName)
        {
            return pathName.Replace(rootPath, "").Replace('\\', '/').TrimStart('/');
        }

    }
}
