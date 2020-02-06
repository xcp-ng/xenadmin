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
using XenAdmin.Network;


namespace XenAdmin.Actions
{

    public abstract class PatchAction : AsyncAction
    {
        public PatchAction(IXenConnection conn, string title) : base(conn, title) { }
        public PatchAction(IXenConnection conn, string title, bool suppress) : base(conn, title, suppress) { }


        protected XenRef<Pool_patch> BringPatchToPoolForHost(Host host, Pool_patch patch)
        {
            // Check the patch exists on the pool this host is connected to
            XenRef<Pool_patch> patch_ref = host.Connection.Cache.FindRef(patch);
            if (patch_ref != null)
                return patch_ref;

            Description = String.Format(Messages.DOWNLOADING_PATCH_FROM, patch.Connection.Name);

            // 1st download patch from the pool that has it (the connection on the xenobject)

            string filename = Path.GetTempFileName();

            try
            {
                Connection = patch.Connection;
                Session = patch.Connection.DuplicateSession();

                try
                {
                    HTTPHelper.Get(this, true, filename, patch.Connection.Hostname,
                        (HTTP_actions.get_sss)HTTP_actions.get_pool_patch_download,
                        Session.opaque_ref, patch.uuid);
                }
                catch (Exception e)
                {
                    throw new PatchDownloadFailedException(string.Format(Messages.PATCH_DOWNLOAD_FAILED, patch.name_label, patch.Connection.Name), e);
                }
                finally
                {
                    Connection = null;
                    Session = null;
                }

                // Then, put it on the pool that doesn't have it

                Description = String.Format(Messages.UPLOADING_PATCH_TO, host.Name());

                Connection = host.Connection;
                Session = host.Connection.DuplicateSession();

                try
                {
                    string result = HTTPHelper.Put(this, true, filename, host.Connection.Hostname,
                        (HTTP_actions.put_ss)HTTP_actions.put_pool_patch_upload, Session.opaque_ref);

                    return new XenRef<Pool_patch>(result);
                }
                finally
                {
                    Connection = null;
                    Session = null;
                    Description = String.Format(Messages.PATCH_UPLOADED, host.Name());
                }
            }
            finally
            {
                File.Delete(filename);
            }
        }
    }

    public class ApplyPatchAction : PatchAction
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly Pool_patch patch;
        private readonly Host host;

        public ApplyPatchAction(Pool_patch patch, Host host)
            : base(host.Connection, string.Format(Messages.UPDATES_WIZARD_APPLYING_UPDATE, patch.Name(), host.Name()))
        {
            this.patch = patch;
            this.host = host;
        }

        protected override void Run()
        {
            SafeToExit = false;

            if (patch.AppliedOn(host) != DateTime.MaxValue)
                return;

            XenRef<Pool_patch> patchRef = BringPatchToPoolForHost(host, patch);

            Description = string.Format(Messages.APPLYING_PATCH, patch.Name(), host.Name());
            log.DebugFormat("Applying update '{0}' to server '{1}'", patch.Name(), host.Name());

            RelatedTask = Pool_patch.async_apply(Session, patchRef, host.opaque_ref);
            PollToCompletion();

            log.DebugFormat("Applied update '{0}' to server '{1}'. Result: {2}.", patch.Name(), host.Name(), Result);
            Description = string.Format(Messages.PATCH_APPLIED, patch.Name(), host.Name());
        }
    }

    public class PatchDownloadFailedException : ApplicationException
    {
        public PatchDownloadFailedException(string message, Exception innerException) :
            base(message, innerException) { }
    }
}
