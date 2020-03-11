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

using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using DiscUtils.Iso9660;
using XenAdmin.Actions;
using XenAdmin.Core;
using XenAdmin.Dialogs;
using XenAPI;


namespace XenAdmin.Wizards
{
    public static class WizardHelpers
    {
        public static string GetSuppPackFromDisk(Control control)
        {
            string oldDir = string.Empty;
            try
            {
                oldDir = Directory.GetCurrentDirectory();
                using (var dlg = new OpenFileDialog
                {
                    Multiselect = false,
                    ShowReadOnly = false,
                    Filter = string.Format(Messages.PATCHINGWIZARD_SELECTPATCHPAGE_UPDATESEXT, BrandManager.ExtensionUpdate),
                    FilterIndex = 0,
                    CheckFileExists = true,
                    CheckPathExists = true,
                    ShowHelp = false,
                    Title = Messages.PATCHINGWIZARD_SELECTPATCHPAGE_CHOOSE
                })
                {
                    dlg.FileOk += dlg_FileOk;

                    if (dlg.ShowDialog(control) == DialogResult.OK)
                        return dlg.FileName;
                }
            }
            finally
            {
                Directory.SetCurrentDirectory(oldDir);
            }

            return null;
        }

        private static void dlg_FileOk(object sender, CancelEventArgs e)
        {
            var dlg = sender as OpenFileDialog;
            if (dlg == null)
                return;

            if (!IsValidFile(dlg.FileName, out var failureReason))
                using (var popup = new ThreeButtonDialog(new ThreeButtonDialog.Details(
                    SystemIcons.Error, failureReason, Messages.UPDATES)))
                {
                    popup.ShowDialog();
                    e.Cancel = true;
                }
        }

        public static string ParseSuppPackFile(string path, Control control, ref bool cancel)
        {
            if (!IsValidFile(path, out var pathFailure))
                using (var dlg = new ThreeButtonDialog(new ThreeButtonDialog.Details(
                    SystemIcons.Error, pathFailure, Messages.UPDATES)))
                {
                    cancel = true;
                    dlg.ShowDialog();
                    return null;
                }

            if (Path.GetExtension(path).ToLowerInvariant().Equals(".zip"))
            {
                var unzippedPath = ExtractUpdate(path, control);

                if (!IsValidFile(unzippedPath, out var zipFailure))
                    using (var dlg = new ThreeButtonDialog(new ThreeButtonDialog.Details(
                        SystemIcons.Error, zipFailure, Messages.UPDATES)))
                    {
                        cancel = true;
                        dlg.ShowDialog();
                        return null;
                    }

                return unzippedPath;
            }

            return path;
        }

        public static string ExtractUpdate(string zippedUpdatePath, Control control)
        {
            if (string.IsNullOrEmpty(zippedUpdatePath))
                return null;

            var unzipAction = new DownloadAndUnzipXenServerPatchAction(Path.GetFileNameWithoutExtension(zippedUpdatePath),
                null, zippedUpdatePath, true, BrandManager.ExtensionUpdate, InvisibleMessages.ISO_UPDATE);

            using (var dlg = new ActionProgressDialog(unzipAction, ProgressBarStyle.Marquee))
            {
                dlg.ShowDialog(control);
            }

            return !string.IsNullOrEmpty(unzipAction.PatchPath) ? unzipAction.PatchPath : null;
        }

        public static bool IsValidFile(string fileName, out string failureReason)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                failureReason = Messages.UPDATES_WIZARD_INVALID_FILE;
                return false;
            }

            if (!File.Exists(fileName))
            {
                failureReason = string.Format(Messages.UPDATES_WIZARD_FILE_NOT_FOUND, fileName);
                return false;
            }

            bool isValidExt = fileName.ToLowerInvariant().EndsWith("." + BrandManager.ExtensionUpdate.ToLowerInvariant())
                              || fileName.ToLowerInvariant().EndsWith(".zip")
                              || fileName.ToLowerInvariant().EndsWith(".iso");

            if (!isValidExt)
            {
                failureReason = string.Format(Messages.UPDATES_WIZARD_INVALID_EXTENSION, BrandManager.ExtensionUpdate);
                return false;
            }

            if (fileName.ToLowerInvariant().EndsWith(".iso"))
            {
                bool isValidIso;
                try
                {
                    using (var isoStream = File.OpenRead(fileName))
                    using (var cd = new CDReader(isoStream, true))
                        isValidIso = cd.Root.Exists;
                }
                catch
                {
                    isValidIso = false;
                }

                if (!isValidIso)
                {
                    failureReason = Messages.UPDATES_WIZARD_INVALID_ISO_FILE;
                    return false;
                }
            }
            failureReason = null;
            return true;
        }

        public static bool IsHostRebootRequiredForUpdate(Host host, Pool_patch patch, Dictionary<string, livepatch_status> livePatchCodesByHost = null)
        {
            return patch.after_apply_guidance.Contains(after_apply_guidance.restartHost)
                   && (livePatchCodesByHost == null || !livePatchCodesByHost.ContainsKey(host.uuid) || livePatchCodesByHost[host.uuid] != livepatch_status.ok_livepatch_complete);
        }

        public static bool IsHostRebootRequiredForUpdate(Host host, Pool_update poolUpdate, Dictionary<string, livepatch_status> livePatchCodesByHost = null)
        {
            return poolUpdate.after_apply_guidance.Contains(update_after_apply_guidance.restartHost)
                   && (livePatchCodesByHost == null || !livePatchCodesByHost.ContainsKey(host.uuid) || livePatchCodesByHost[host.uuid] != livepatch_status.ok_livepatch_complete);
        }

    }
}
