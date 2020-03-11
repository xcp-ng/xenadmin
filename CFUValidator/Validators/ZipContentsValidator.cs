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
using System.Linq;
using System.Collections.Generic;
using System.IO;
using XenAdmin;
using XenAdmin.Actions;
using XenAdmin.Alerts;
using XenAdmin.Core;
using XenCenterLib.Archive;

namespace CFUValidator.Validators
{
    class ZipContentsValidator : AlertFeatureValidator
    {
        public ZipContentsValidator(List<XenServerPatchAlert> alerts) : base(alerts){}

        public override void Validate()
        {
            foreach (XenServerPatchAlert alert in alerts.OrderBy(a=>a.Patch.Name))
            {
                DownloadPatchFile(alert);
            }
            
        }

        public override string Description
        {
            get { return "Downloading and checking the contents of the zip files in the patch"; }
        }

        private string NewTempPath()
        {
            return Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        }

        private void DownloadPatchFile(XenServerPatchAlert patch)
        {
            if(string.IsNullOrEmpty(patch.Patch.PatchUrl))
            {
                Results.Add("Patch conatined no URL: " + patch.Patch.Name);
                return;
            }

            string tempFileName = NewTempPath();
            DownloadAndUnzipXenServerPatchAction action = new DownloadAndUnzipXenServerPatchAction(patch.Patch.Name,
                new Uri(patch.Patch.PatchUrl),
                tempFileName, false, BrandManager.ExtensionUpdate, InvisibleMessages.ISO_UPDATE);

            try
            {
                Status = "Download and unzip patch " + patch.Patch.Name;

                ConsoleSpinner spinner = new ConsoleSpinner();
                action.RunAsync();
                while(!action.IsCompleted)
                {
                    spinner.Turn(action.PercentComplete);
                }
                
                if(!action.Succeeded)
                    Results.Add("Patch download and unzip unsuccessful: " + action.Exception.Message);
            }
            catch (Exception ex)
            {
                Results.Add("Patch download error: " + ex.Message);
            }

        }
    }
}
