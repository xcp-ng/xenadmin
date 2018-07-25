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
using System.Reflection;
using XenAdmin.Network;
using XenAdmin.Core;
using XenAPI;


namespace XenAdmin.Actions
{
    public class UploadPatchAction : AsyncAction, IByteProgressAction
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private Pool_patch _patch;

        private readonly string retailPatchPath;
        private readonly string _patchName;
        private readonly bool deleteFileOnCancel;
        private readonly long _totalPatchSize;


        public UploadPatchAction(IXenConnection connection, string path, bool suppressHistory, bool deleteFileOnCancel)
            : base(connection, null, Messages.UPLOADING_PATCH, suppressHistory)
        {
            Host master = Helpers.GetMaster(connection);
            this.deleteFileOnCancel = deleteFileOnCancel;
            if (master == null)
                throw new NullReferenceException();

            ApiMethodsToRoleCheck.Add("pool.sync_database");
            ApiMethodsToRoleCheck.Add("http/put_pool_patch_upload");

            retailPatchPath = path;
            _patchName = Path.GetFileNameWithoutExtension(retailPatchPath);
            _totalPatchSize = (new FileInfo(path)).Length;
            Host = master;
        }

        public string ByteProgressDescription { get; set; }

        public Pool_patch Patch
        {
            get { return _patch; }
        }

        protected override void Run()
        {
            SafeToExit = false;

            try
            {
                _patch = UploadRetailPatch();
                log.InfoFormat("File '{0}' uploaded to server '{1}'", _patchName, Host.Name());
                Tick(100, string.Format(Messages.UPLOAD_PATCH_UPLOADED_DESCRIPTION, _patchName));
            }
            catch (Failure f)
            {
                // Need to check if the patch already exists.  
                // If it does then we use it and ignore the PATCH_ALREADY_EXISTS error (CA-110209).
                if (f.ErrorDescription != null
                    && f.ErrorDescription.Count > 1
                    && f.ErrorDescription[0] == Failure.PATCH_ALREADY_EXISTS)
                {
                    string uuid = f.ErrorDescription[1];
                    Session session = Host.Connection.DuplicateSession();
                    _patch = Connection.Resolve(Pool_patch.get_by_uuid(session, uuid));
                }
                else
                    throw;
            }
        }

        public override void RecomputeCanCancel()
        {
            CanCancel = !Cancelling;
        }

        private Pool_patch UploadRetailPatch()
        {
            Session session = NewSession();

            Host master = Helpers.GetMaster(Connection);

            log.InfoFormat("Uploading file '{0}' to server '{1}'", _patchName, master.Name());
            this.Description = string.Format(Messages.UPLOAD_PATCH_UPLOADING_DESCRIPTION, _patchName);

            RelatedTask = Task.create(session, "uploadTask", retailPatchPath);

            try
            {
                var result = HTTPHelper.Put(UpdateProgress, GetCancelling, true, Connection, RelatedTask, ref session, retailPatchPath,
                    master.address, (HTTP_actions.put_ss)HTTP_actions.put_pool_patch_upload, session.opaque_ref);

                return Connection.WaitForCache(new XenRef<Pool_patch>(result));
            }
            catch (CancelledException)
            {
                if (deleteFileOnCancel && File.Exists(retailPatchPath))
                {
                    File.Delete(retailPatchPath);
                }
                throw;
            }
            catch (TargetInvocationException ex)
            {
                if (ex.InnerException != null)
                    throw ex.InnerException;
                else
                    throw;
            }
            finally
            {
                Task.destroy(session, RelatedTask);
                RelatedTask = null;
            }
        }

        private void UpdateProgress(int percent)
        {
            var descr = string.Format(Messages.UPLOAD_PATCH_UPLOADING_PROGRESS_DESCRIPTION, _patchName,
                Util.DiskSizeString(percent * _totalPatchSize / 100, "F1"), Util.DiskSizeString(_totalPatchSize));

            ByteProgressDescription = descr;
            Tick(percent, descr);
        }
    }

    public interface IByteProgressAction
    {
        string ByteProgressDescription { get; set; }
    }
}
