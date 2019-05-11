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
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using XenAdmin.Actions;
using XenAdmin.Dialogs;
using XenAPI;


namespace XenAdmin.Commands
{
    class ApplyLicenseEditionCommand : Command
    {
        private readonly IEnumerable<IXenObject> xos;
        private readonly Host.Edition _edition;
        private readonly string _licenseServerAddress;
        private readonly string _licenseServerPort;

        public event EventHandler<EventArgs> Succedded;

        public void InvokeSuccedded(EventArgs e)
        {
            EventHandler<EventArgs> handler = Succedded;
            if (handler != null) handler(this, e);
        }


        public ApplyLicenseEditionCommand(IMainWindow mainWindow, IEnumerable<IXenObject> xos,
            Host.Edition edition, string licenseServerAddress, string licenseServerPort,
            Control parent)
        : base(mainWindow)
        {
            _edition = edition;
            this.xos = xos;
            _licenseServerAddress = licenseServerAddress;
            _licenseServerPort = licenseServerPort;
            SetParent(parent);
        }

        protected override void ExecuteCore(SelectedItemCollection selection)
        {
            ApplyLicenseEditionAction action = new ApplyLicenseEditionAction(xos, _edition, _licenseServerAddress, _licenseServerPort, null);
            using (var actionProgress = new ActionProgressDialog(action, ProgressBarStyle.Marquee))
            {
                //ActionProgressDialog closureDialog = actionProgress;
                // close dialog even when there's an error as this action shows its own error dialog box.
                action.Completed += s =>
                {
                    Program.Invoke(Program.MainWindow, () =>
                    {
                        Failure f = action.Exception as Failure;
                        if (f != null && f.ErrorDescription[0] == Failure.RBAC_PERMISSION_DENIED_FRIENDLY)
                            return;

                        actionProgress.Close();
                    });

                    if (action.Exception != null)
                    {
                        ShowLicensingFailureDialog(action.LicenseFailures, action.Exception.Message, Parent);
                    }
                };

                actionProgress.ShowDialog(Parent);

                if (actionProgress.action.Succeeded)
                {
                    InvokeSuccedded(null);
                }
            }
        }

        public static void ShowLicensingFailureDialog(List<LicenseFailure> licenseFailures, string exceptionMessage)
        {
            ShowLicensingFailureDialog(licenseFailures, exceptionMessage, Program.MainWindow);
        }

        public static void ShowLicensingFailureDialog(List<LicenseFailure> licenseFailures, string exceptionMessage, Control parent)
        {
            Debug.Assert(licenseFailures.Count > 0);


             if (licenseFailures.Count == 1)
             {
                 Program.Invoke(Program.MainWindow, () =>
                 {
                     using (var dlg = new ThreeButtonDialog(new ThreeButtonDialog.Details(SystemIcons.Error, licenseFailures[0].AlertText,
                                                                Messages.LICENSE_ERROR_TITLE),
                                                            ThreeButtonDialog.ButtonOK))
                     {
                         dlg.ShowDialog(parent);
                     }
                 });
             }
             else
             {
                 var failureDic = new Dictionary<IXenObject, string>();

                 foreach (var f in licenseFailures)
                 {
                     failureDic.Add(f.Host, f.AlertText);
                 }

                 Program.Invoke(Program.MainWindow, () => 
                 {
                     using (var dlg = new CommandErrorDialog(Messages.LICENSE_ERROR_TITLE, exceptionMessage, failureDic))
                     {
                         dlg.ShowDialog(parent);
                     }
                 });
             }
        }
    }
}
