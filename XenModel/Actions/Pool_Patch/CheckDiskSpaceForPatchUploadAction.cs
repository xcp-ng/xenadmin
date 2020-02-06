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
using XenAPI;


namespace XenAdmin.Actions
{
    public class CheckDiskSpaceForPatchUploadAction : PureAsyncAction
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly string fileName;
        private readonly long fileSize;

        /// <summary>
        /// This constructor is used to check disk space for uploading a single update file
        /// </summary>
        public CheckDiskSpaceForPatchUploadAction(Host host, string path, bool suppressHistory)
            : this(host, FileName(path), FileSize(path), suppressHistory)
        { }

        /// <summary>
        /// This constructor is used to check disk space for uploading a file of given size
        /// </summary>
        public CheckDiskSpaceForPatchUploadAction(Host host, string fileName, long size, bool suppressHistory)
            : base(host.Connection, Messages.ACTION_CHECK_DISK_SPACE_TITLE, "", suppressHistory)
        {
            if (host == null)
                throw new ArgumentNullException("host");
            Host = host;
            this.fileName = fileName;
            fileSize = size;
        }

        private static long FileSize(string path)
        {
            FileInfo fileInfo = new FileInfo(path);
            return fileInfo.Length;
        }

        private static string FileName(string path)
        {
            FileInfo fileInfo = new FileInfo(path);
            return fileInfo.Name;
        }

        protected override void Run()
        {
            SafeToExit = false;
            Description = String.Format(Messages.ACTION_CHECK_DISK_SPACE_DESCRIPTION, Host.Name());
            if (!IsEnoughDiskSpace())
            {
                DiskSpaceRequirements diskSpaceRequirements = null;
                var getDiskSpaceRequirementsAction = new GetDiskSpaceRequirementsAction(Host, fileName, fileSize, true);
                try
                {
                    getDiskSpaceRequirementsAction.RunExternal(Session);
                    diskSpaceRequirements = getDiskSpaceRequirementsAction.DiskSpaceRequirements;
                }
                catch (Failure failure)
                {
                    log.Warn($"Getting disk space requirements on {Host.Name()} failed.", failure);
                }
                throw new NotEnoughSpaceException(Host.Name(), fileName, diskSpaceRequirements);
            }
        }

        private bool IsEnoughDiskSpace()
        {
            string result;
            try
            {
                result = Host.call_plugin(Session, Host.opaque_ref, "disk-space", "check_patch_upload", 
                    new Dictionary<string, string> { { "size", fileSize.ToString() } });
            }
            catch (Failure failure)
            {
                log.WarnFormat("Plugin call disk-space.check_patch_upload({0}) on {1} failed with {2}", fileSize, Host.Name(),
                               failure.Message);
                return true;
            }
            return result.ToLower() == "true";
        }
    }

    public class NotEnoughSpaceException : Exception
    {
        private readonly string host;
        private readonly string fileName;

        public DiskSpaceRequirements DiskSpaceRequirements { get; private set; }

        public NotEnoughSpaceException(string host, string fileName, DiskSpaceRequirements diskSpaceRequirements)
        {
            this.host = host;
            this.fileName = fileName;
            DiskSpaceRequirements = diskSpaceRequirements;
        }

        public override string Message
        {
            get { return String.Format(Messages.NOT_ENOUGH_SPACE_MESSAGE_UPLOAD, host, fileName); }
        }
    }
}

