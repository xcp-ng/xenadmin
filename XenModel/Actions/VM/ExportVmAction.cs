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
using System.ComponentModel;
using XenAdmin.Core;
using System.IO;
using XenAdmin.Network;
using System.Threading;
using XenAPI;
using XenCenterLib.Archive;
using XenCenterLib;

namespace XenAdmin.Actions
{
    public class ExportVmAction : AsyncAction
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly string _filename;
        private Exception _exception = null;
        private Export export;
        private readonly bool verify;

		/// <summary>
		/// RBAC dependencies needed to import appliance/export an appliance/import disk image.
		/// </summary>
		public static RbacMethodList StaticRBACDependencies
		{
			get
			{
				var list = new RbacMethodList("task.create", "http/get_export");
				list.AddRange(Role.CommonTaskApiList);
				list.AddRange(Role.CommonSessionApiList);
				return list;
			}
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="host">Used for filtering purposes. May be null.</param>
        /// <param name="vm"></param>
        /// <param name="filename"></param>
        /// <param name="verify"></param>
        public ExportVmAction(IXenConnection connection, Host host,
            VM vm, string filename, bool verify)
            : base(connection, string.Format(Messages.EXPORT_VM_TITLE, vm.Name(), Helpers.GetName(connection)),
            Messages.ACTION_EXPORT_DESCRIPTION_PREPARING)
        {
            #region RBAC Dependencies
            ApiMethodsToRoleCheck.Add("task.create");
            ApiMethodsToRoleCheck.AddRange(Role.CommonTaskApiList);
            ApiMethodsToRoleCheck.AddRange(Role.CommonSessionApiList);

            ApiMethodsToRoleCheck.Add("http/get_export");
            #endregion

            this.Pool = Helpers.GetPool(vm.Connection);
            this.Host = host;
            this.VM = vm;
            _filename = filename;
            this.verify = verify;
        }

        protected override void Run()
        {
            SafeToExit = false;
            Description = Messages.ACTION_EXPORT_DESCRIPTION_IN_PROGRESS;

            RelatedTask = XenAPI.Task.create(Session,
                string.Format(Messages.ACTION_EXPORT_TASK_NAME, VM.Name()),
                string.Format(Messages.ACTION_EXPORT_TASK_DESCRIPTION, VM.Name()));

            UriBuilder uriBuilder = new UriBuilder(this.Session.Url);
            uriBuilder.Path = "export";
            uriBuilder.Query = string.Format("session_id={0}&uuid={1}&task_id={2}",
                Uri.EscapeDataString(this.Session.opaque_ref),
                Uri.EscapeDataString(this.VM.uuid),
                Uri.EscapeDataString(this.RelatedTask.opaque_ref));

            log.DebugFormat("Exporting {0} to {1}", VM.Name(), _filename);

            // The DownloadFile call will block, so we need a separate thread to poll for task status.
            Thread taskThread = new Thread(progressPoll);
            taskThread.Name = "Progress polling thread for ExportVmAction for " + VM.Name().Ellipsise(20);
            taskThread.IsBackground = true;
            taskThread.Start();

            // Create the file with a temporary name till it is fully downloaded
            String tmpFile = _filename + ".tmp";
            try
            {
                HttpGet(tmpFile, uriBuilder.Uri);
            }
            catch (Exception e)
            {
                if (XenAPI.Task.get_status(this.Session, this.RelatedTask.opaque_ref) == XenAPI.task_status_type.pending
                    && XenAPI.Task.get_progress(this.Session, this.RelatedTask.opaque_ref) == 0)
                {
                    // If task is pending and has zero progress, it probably hasn't been started,
                    // which probably means there was an exception in the GUI code before the
                    // action got going. Kill the task so that we don't block forever on
                    // taskThread.Join(). Brought to light by CA-11100.
                    DestroyTask();
                }
                // Test for null: don't overwrite a previous exception
                if (_exception == null)
                    _exception = e;
            }

            taskThread.Join();

            using (FileStream fs = new FileStream(tmpFile, FileMode.Append))
            {
                // Flush written data to disk
                if (!Win32.FlushFileBuffers(fs.SafeFileHandle))
                {
                    Win32Exception exn = new Win32Exception(System.Runtime.InteropServices.Marshal.GetLastWin32Error());
                    log.Error(string.Format("FlushFileBuffers failed in ExportVmAction with NativeErrorCode={0}",
                        exn.NativeErrorCode), exn);
                }
            }

            if (verify && _exception == null)
            {
                long read = 0;
                int i = 0;
                long filesize = new FileInfo(tmpFile).Length / 50; //Div by 50 to save doing the * 50 in the callback

                Export.verifyCallback callback = size =>
                    {
                        read += size;
                        i++;

                        //divide number of updates by 10, so as not to spend all out time redrawing the control
                        //but try and send an update every second to keep the timer ticking
                        if (i > 10)
                        {
                            PercentComplete = 50 + (int)(read / filesize);
                            i = 0;
                        }
                    };

                try
                {
                    using (FileStream fs = new FileStream(tmpFile, FileMode.Open, FileAccess.Read))
                    {
                        log.DebugFormat("Verifying export of {0} in {1}", VM.Name(), _filename);
                        this.Description = Messages.ACTION_EXPORT_VERIFY;

                        export = new Export();
                        export.verify(fs, null, () => Cancelling, callback);
                    }
                }
                catch (Exception e)
                {
                    if (_exception == null)
                        _exception = e;
                }
            }

            if (Cancelling || _exception is CancelledException)
            {
                log.InfoFormat("Export of VM {0} cancelled", VM.Name());
                this.Description = Messages.ACTION_EXPORT_DESCRIPTION_CANCELLED;

                log.DebugFormat("Deleting {0}", tmpFile);
                File.Delete(tmpFile);
                throw new CancelledException();
            }
            else if (_exception != null)
            {
                log.Warn(string.Format("Export of VM {0} failed", VM.Name()), _exception);

                if (_exception is HeaderChecksumFailed || _exception is FormatException)
                    this.Description = Messages.ACTION_EXPORT_DESCRIPTION_HEADER_CHECKSUM_FAILED;
                else if (_exception is BlockChecksumFailed)
                    this.Description = Messages.ACTION_EXPORT_DESCRIPTION_BLOCK_CHECKSUM_FAILED;
                else if (_exception is IOException && Win32.GetHResult(_exception) == Win32.ERROR_DISK_FULL)
                    this.Description = Messages.ACTION_EXPORT_DESCRIPTION_DISK_FULL;
                else if (_exception is Failure && ((Failure)_exception).ErrorDescription[0] == Failure.VDI_IN_USE)
                    this.Description = Messages.ACTION_EXPORT_DESCRIPTION_VDI_IN_USE;
                else
                    this.Description = Messages.ACTION_EXPORT_DESCRIPTION_FAILED;

                var fi = new FileInfo(tmpFile);
                log.DebugFormat("Progress of the action until exception: {0}", PercentComplete);
                log.DebugFormat("Size file exported until exception: {0}", fi.Length);

                try
                {
                    using (Stream stream = new FileStream(tmpFile, FileMode.Open, FileAccess.Read))
                    using (var iterator = ArchiveFactory.Reader(ArchiveFactory.Type.Tar, stream))
                    {
                        while (iterator.HasNext())
                            log.DebugFormat("Tar entry: {0} {1}", iterator.CurrentFileName(), iterator.CurrentFileSize());
                    }
                }
                catch (Exception)
                {}

                log.DebugFormat("Deleting {0}", tmpFile);
                File.Delete(tmpFile);
                throw new Exception(Description);
            }
            else
            {
                log.InfoFormat("Export of VM {0} successful", VM.Name());
                this.Description = Messages.ACTION_EXPORT_DESCRIPTION_SUCCESSFUL;

                log.DebugFormat("Renaming {0} to {1}", tmpFile, _filename);
                if (File.Exists(_filename))
                    File.Delete(_filename);
                File.Move(tmpFile, _filename);
            }
        }



        private void HttpGet(string filename, Uri uri)
        {
            using (FileStream fs = new FileStream(filename, FileMode.Create, FileAccess.Write))
            {
                using (Stream http = HTTPHelper.GET(uri, Connection, true))
                {
                    new Export().verify(http, fs, () => Cancelling);
                }
            }
        }

        private void progressPoll()
        {
            try
            {
                PollToCompletion(0, verify ? 50 : 95);
            }
            catch (Failure e)
            {
                // Don't overwrite a previous exception unless we're sure that the one that
                // we have here is going to be more useful than the client one.  Sometimes,
                // the server exception will be "failed to write", which is just in
                // response to us closing the stream when we run out of disk space or whatever
                // on the client side.  Other times, it's the server that's got the useful
                // error message.
                if (_exception == null || e.ErrorDescription[0] == Failure.VDI_IN_USE)
                    _exception = e;
            }
            catch (Exception e)
            {
                // Test for null: don't overwrite a previous exception
                if (_exception == null)
                    _exception = e;
            }
        }
    }
}
